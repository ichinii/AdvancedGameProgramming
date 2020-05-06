using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipable : MonoBehaviour
{
    [SerializeField] private SpringJoint m_flipJoint;
    [SerializeField] private Vector3 m_prevLocalPosition;
    [SerializeField] private Quaternion m_prevLocalRotation;
    [SerializeField] private Vector3 m_prevLocalScale;

    public bool isFlipable {
        get { return GetComponent<Rigidbody>().IsSleeping(); }
    }

    void Start()
    {
        // make sure we have valid stand position to start with, though it might be invalid. should never be read actually
        noticeStandPosition();
    }

    void Update()
    {
        var rigidBody = GetComponent<Rigidbody>();
        if (rigidBody.IsSleeping()) {

            // are we standing on our bottom? is the center of mass between our left and right foot
            var center = rigidBody.worldCenterOfMass;
            var leftFoot = transform.position - transform.up * 0.5f - transform.right * 0.25f;
            var rightFoot = transform.position - transform.up * 0.5f + transform.right * 0.25f;
            bool isStanding = center.x > leftFoot.x && center.x < rightFoot.x;

            if (isStanding) {
                noticeStandPosition();
            } else {
                resetPosition();
            }
        }
    }

    public void createFlipJoint(Rigidbody connectedBody, Vector3 point)
    {
        // be on the save side and release flipjoint
        releaseFlipJoint();

        var rigidBody = GetComponent<Rigidbody>();
        m_flipJoint = gameObject.AddComponent<SpringJoint>();
        m_flipJoint.name = "FlipJoint";
        m_flipJoint.connectedBody = connectedBody;
        m_flipJoint.axis = new Vector3(0, 0, 1);
        m_flipJoint.autoConfigureConnectedAnchor = false;
        m_flipJoint.anchor = transform.worldToLocalMatrix * new Vector4(point.x, point.y, point.z, 1);
        m_flipJoint.connectedAnchor = new Vector3(0, 0, 0);
    }

    public void releaseFlipJoint()
    {
        if (m_flipJoint)
            Destroy(m_flipJoint);
        m_flipJoint = null;
    }

    public void noticeStandPosition()
    {
        m_prevLocalPosition = transform.localPosition;
        m_prevLocalRotation = transform.localRotation;
        m_prevLocalScale = transform.localScale;
    }

    public void resetPosition()
    {
        transform.localPosition = m_prevLocalPosition;
        transform.localRotation = m_prevLocalRotation;
        transform.localScale = m_prevLocalScale;
    }

    private void OnDrawGizmos()
    {
        if (m_flipJoint != null) {
            var flipJointAnchor = new Vector4(m_flipJoint.anchor.x, m_flipJoint.anchor.y, m_flipJoint.anchor.z, 1.0f);
            Debug.DrawLine(transform.localToWorldMatrix * flipJointAnchor, m_flipJoint.connectedBody.position, Color.red);
        }
    }
}
