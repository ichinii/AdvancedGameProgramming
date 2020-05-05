using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipable : MonoBehaviour
{
    [SerializeField] private SpringJoint m_flipJoint;

    public bool isFlipable {
        get { return GetComponent<Rigidbody>().IsSleeping(); }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (m_flipJoint != null)
            Debug.DrawLine(transform.localToWorldMatrix * v4(m_flipJoint.anchor), m_flipJoint.connectedBody.position, Color.red);
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

    public void releaseFlipJoint() {
        if (m_flipJoint)
            Destroy(m_flipJoint);
        m_flipJoint = null;
    }

    public Vector4 v4(Vector3 v) {
        return new Vector4(v.x, v.y, v.z, 1);
    }
}
