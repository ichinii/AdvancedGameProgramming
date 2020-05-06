﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipable : MonoBehaviour
{
    [SerializeField] private SpringJoint m_flipJoint;
    [SerializeField] private Vector3 m_prevLocalPosition;
    [SerializeField] private Quaternion m_prevLocalRotation;
    [SerializeField] private Vector3 m_prevLocalScale;
    [SerializeField] private Vector3 m_prevUp;
    [SerializeField] private float m_standAngle;
    [SerializeField] private float m_flipAngle;
    [SerializeField] private float m_flipAngleSign;
    [SerializeField] private int m_flipCount;

    public bool isFlipable {
        get { return IsStanding(); }
    }

    void Start()
    {
        // make sure we have valid stand position to start with, though it might be invalid. should never be read actually
        NoticeStandPosition();
        ResetFlipCountValid();
        m_prevUp = transform.up;
        m_flipAngleSign = 1.0f;
    }

    void Update()
    {
        var rigidBody = GetComponent<Rigidbody>();

        // accept new position only if we are standing and were flipped
        if (IsStanding() && WasFlipped()) {
            NoticeStandPosition();
        
        // else reset to previous position
        } else if (rigidBody.IsSleeping()) {
            ResetStandPosition();
            ResetFlipCountValid();
        }
    }

    void FixedUpdate()
    {
        DetectFlips();
        m_prevUp = transform.up;
    }

    public void CreateFlipJoint(Rigidbody connectedBody, Vector3 point)
    {
        // be on the save side and release flipjoint
        ReleaseFlipJoint();

        var rigidBody = GetComponent<Rigidbody>();
        m_flipJoint = gameObject.AddComponent<SpringJoint>();
        m_flipJoint.name = "FlipJoint";
        m_flipJoint.connectedBody = connectedBody;
        m_flipJoint.axis = new Vector3(0, 0, 1);
        m_flipJoint.autoConfigureConnectedAnchor = false;
        m_flipJoint.anchor = transform.worldToLocalMatrix * new Vector4(point.x, point.y, point.z, 1);
        m_flipJoint.connectedAnchor = new Vector3(0, 0, 0);

        ResetFlipCountInvalid();
    }

    public void ReleaseFlipJoint()
    {
        if (m_flipJoint)
            Destroy(m_flipJoint);
        m_flipJoint = null;
    }

    public void NoticeStandPosition()
    {
        m_prevLocalPosition = transform.localPosition;
        m_prevLocalRotation = transform.localRotation;
        m_prevLocalScale = transform.localScale;
    }

    public void ResetStandPosition()
    {
        transform.localPosition = m_prevLocalPosition;
        transform.localRotation = m_prevLocalRotation;
        transform.localScale = m_prevLocalScale;
    }

    public bool IsStanding()
    {
        var rigidBody = GetComponent<Rigidbody>();

        // are we upright / standing on our bottom? see if the center of mass is between our left and right foot
        var center = rigidBody.worldCenterOfMass;
        var leftFoot = transform.position - transform.up * 0.5f - transform.right * 0.25f;
        var rightFoot = transform.position - transform.up * 0.5f + transform.right * 0.25f;
        bool isUpright = center.x > leftFoot.x && center.x < rightFoot.x;

        return rigidBody.IsSleeping() && isUpright;
    }

    public bool WasFlipped()
    {
        return m_flipCount > 0;
    }

    public void ResetFlipCountInvalid()
    {
        m_standAngle = transform.rotation.z;
        m_flipAngle = 0.0f;
        m_flipCount = 0;
    }

    public void ResetFlipCountValid()
    {
        ResetFlipCountInvalid();
        m_flipCount = 1;
    }

    public void DetectFlips()
    {
        // get the direction of the flip
        var sign = Mathf.Sign(Vector3.Dot(transform.right, m_prevUp));
        Debug.Log(Vector3.Dot(transform.right, m_prevUp));

        // reset the flip angle if the direction changes
        if (sign != m_flipAngleSign) {
            m_flipAngle = 0.0f;
            //Debug.Log(sign);
            m_flipAngleSign = sign;
        }

        // angle since the last frame
        var angle = Vector3.Dot(transform.up, m_prevUp);
        angle = Mathf.Clamp(angle, -1.0f, 1.0f); // somehow those limits get exceeded even with unit vectors
        angle = Mathf.Acos(angle);
        m_flipAngle += angle;

        var threshold = Mathf.PI * 2.0f * 0.95f;

        // detect 360 degrees flips
        if (m_flipAngle >= threshold) {
            ++m_flipCount;
            m_flipAngle -= threshold;
            Debug.Log(m_flipCount);
        }
    }

    private void OnDrawGizmos()
    {
        if (m_flipJoint != null) {
            var flipJointAnchor = new Vector4(m_flipJoint.anchor.x, m_flipJoint.anchor.y, m_flipJoint.anchor.z, 1.0f);
            Debug.DrawLine(transform.localToWorldMatrix * flipJointAnchor, m_flipJoint.connectedBody.position, Color.red);
        }
    }
}
