using System.Collections;
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

    public float m_footHorizontalOffset = 0.5f;
    public float m_footVerticalOffset = -1.0f;

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

        // valid standing on ground
        if (IsStanding() && WasFlipped()) {
            // accept new position only if we are standing and were flipped
            NoticeStandPosition();

            // indicate that body is ready to be flipped
            var material = GetComponent<MeshRenderer>().material;
            material.SetColor("_BaseColor", new Color(0x55 / 255.0f, 0xFA / 255.0f, 0xBE / 255.0f, 1.0f));

            // see if level is finished
            TryFinishLevel();
        }
        
        // body not moving but hasn't been flipped correctly
        else if (rigidBody.IsSleeping()) {
            // reset to previous position
            ResetStandPosition();
            ResetFlipCountValid();
        }
        
        // body is flipping right now
        else {
            // indicate that body is not ready to be flipped
            var material = GetComponent<MeshRenderer>().material;
            material.SetColor("_BaseColor", new Color(0.5f, 0.5f, 0.5f, 0xFF));
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
        bool isUpright = center.x > LeftFootPosition().x && center.x < RightFootPosition().x;

        return rigidBody.IsSleeping() && isUpright;
    }

    public Vector3 LeftFootPosition()
    {
        return transform.position + transform.up * m_footVerticalOffset - transform.right * m_footHorizontalOffset;
    }

    public Vector3 RightFootPosition()
    {
        return transform.position + transform.up * m_footVerticalOffset + transform.right * m_footHorizontalOffset;
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
        Debug.Log("ReseFlipCountValid");
        ResetFlipCountInvalid();
        m_flipCount = 1;
    }

    public void DetectFlips()
    {
        // get the direction of the flip
        var sign = Mathf.Sign(Vector3.Dot(transform.right, m_prevUp));

        // reset the flip angle if the direction changes
        if (sign != m_flipAngleSign) {
            m_flipAngle = 0.0f;
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
        }
    }

    private void TryFinishLevel()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit)) {
            if (hit.collider) {
                var goalGround = hit.collider.GetComponent<GoalGround>();
                if (goalGround != null && goalGround.IsGoalGround) {
                    // we finished this level (yay)
                    Debug.Log("goal");
                    StateController.Instance.RotateLevel();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
            StateController.Instance.RotateLevel();
    }

    private void OnDrawGizmos()
    {
        var leftFoot = LeftFootPosition();
        var rightFoot = RightFootPosition();
        var radius =  new Vector3(0, 0, 0.5f);
        Debug.DrawLine(leftFoot - radius, leftFoot + radius, Color.yellow);
        Debug.DrawLine(rightFoot - radius, rightFoot + radius, Color.red);

        if (m_flipJoint != null) {
            var flipJointAnchor = new Vector4(m_flipJoint.anchor.x, m_flipJoint.anchor.y, m_flipJoint.anchor.z, 1.0f);
            Debug.DrawLine(transform.localToWorldMatrix * flipJointAnchor, m_flipJoint.connectedBody.position, Color.red);
        }
    }
}
