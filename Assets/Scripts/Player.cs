using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera m_camera;
    public Cursor m_cursor;
    public Transform m_flipablesContainer;
    [SerializeField] private Transform[] m_flipables;
    public float m_cursorRange = 2.0f;

    void Update()
    {
        // update flipables
        m_flipables = QueryFlipables();

        var closest = ClosestFlipable();
        closest = closest == null || closest.distance > m_cursorRange ? null : closest;

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            // be on the save side and release all flipjoints
            ReleaseFlipJoints();

            if (closest != null) {
                closest.transform.GetComponent<Flipable>().CreateFlipJoint(
                    m_cursor.GetComponent<Rigidbody>(),
                    closest.point);
            }
        }

        // remove flipjoint when releasing
        if (Input.GetKeyUp(KeyCode.Mouse1)) {
            ReleaseFlipJoints();
        }
    }

    private Transform[] QueryFlipables()
    {
        var flipables = new Transform[m_flipablesContainer.childCount];
        var flipableTransforms = m_flipablesContainer.GetComponentsInChildren<Transform>();
        int i = 0;
        foreach (Transform flipable in m_flipablesContainer)
            flipables[i++] = flipable;
        return flipables;
    }

    private class ClosestTransform
    {
        public Transform transform = null;
        public Vector3 point;
        public float distance;
    }

    private ClosestTransform ClosestFlipable()
    {
        var closest = new ClosestTransform();

        foreach (Transform flipable in m_flipables) {
            if (!flipable.GetComponent<Flipable>().isFlipable)
                continue;

            var closestPoint = flipable.GetComponent<Collider>().ClosestPoint(m_cursor.transform.position);
            closestPoint.z = 0.0f; // make sure we stay on the game plane
            var distance = (closestPoint - m_cursor.transform.position).magnitude;

            if (closest.transform == null || distance < closest.distance) {
                closest.transform = flipable;
                closest.point = closestPoint;
                closest.distance = distance;
            }
        }

        return closest.transform ? closest : null;
    }

    private void ReleaseFlipJoints()
    {
        foreach (Transform flipable in m_flipables) {
            flipable.GetComponent<Flipable>().ReleaseFlipJoint();
        }
    }

    private void OnDrawGizmos()
    {
        var closest = ClosestFlipable();
        if (closest != null && closest.distance <= m_cursorRange) {
            Debug.DrawLine(m_cursor.transform.position, closest.point, Color.green);
        }
    }
}
