using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera m_camera;
    public Cursor m_cursor;
    public Transform m_flipablesContainer;
    [SerializeField] private Transform[] m_flipables;

    void Start()
    {

    }

    void Update()
    {
        // update flipables
        m_flipables = queryFlipables();

        var closest = closestFlipable();
        if (closest != null) {
            if (closest.distance > 2.0f) {
                closest = null;
            } else {
                Debug.DrawLine(m_cursor.transform.position, closest.point, Color.green);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            // be on the save side and release all flipjoints
            releaseFlipJoints();

            if (closest != null) {
                closest.transform.GetComponent<Flipable>().createFlipJoint(
                    m_cursor.GetComponent<Rigidbody>(),
                    closest.point);
            }
        }

        // remove flipjoint when releasing
        if (Input.GetKeyUp(KeyCode.Mouse1)) {
            releaseFlipJoints();
        }
    }

    private Transform[] queryFlipables()
    {
        var flipables = new Transform[m_flipablesContainer.childCount];
        var flipableTransforms = m_flipablesContainer.GetComponentsInChildren<Transform>();
        int i = 0;
        foreach (Transform flipable in m_flipablesContainer)
            flipables[i++] = flipable;
        return flipables;
    }

    private class ClosestTransform {
        public Transform transform = null;
        public Vector3 point;
        public float distance;
    }

    private ClosestTransform closestFlipable() {
        var closest = new ClosestTransform();

        foreach (Transform flipable in m_flipables) {
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

    private void releaseFlipJoints() {
        foreach (Transform flipable in m_flipables) {
            flipable.GetComponent<Flipable>().releaseFlipJoint();
        }
    }
}
