using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeReel : MonoBehaviour
{
    public static RopeReel Instance { get; private set; }
    public bool Active => m_attached != null;
    public float CurrentRopeLength => (m_partLength * m_parts.Count) / 2;

    public Transform m_attachOnStart;
    public Joint2D m_playerJoint;

    public float m_partLength;
    public float m_partMass;
    public float m_partThickness;
    public float m_jointLimit;

    private List<Transform> m_parts;

    private Transform m_attached;
    private Transform m_currentRope;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (m_attachOnStart != null)
        {
            Attach(m_attachOnStart);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (m_attached != null)
        {
            var distanceToLastPart = Vector3.Distance(PlayerMovement.Instance.transform.position, m_parts.Last().position);
            if (distanceToLastPart > m_partLength)
            {
                m_playerJoint.connectedBody = CreateLinkPart();
            }
            m_playerJoint.enabled = true;
        }
        else
        {
            m_playerJoint.enabled = false;
        }
    }

    public void Attach(Transform attachTarget)
    {
        m_parts = new List<Transform>();
        m_currentRope = new GameObject("Rope").transform;
        m_currentRope.transform.position = attachTarget.position;
        var ropeRenderer = m_currentRope.gameObject.AddComponent<RopeRenderer>();
        ropeRenderer.Setup(m_parts, m_partThickness);
        m_attached = attachTarget;

        m_parts.Add(CreateFirstPart());
        CreateLinkPart();
    }

    private Rigidbody2D CreateLinkPart()
    {
        var part = CreatePart();
        var target = m_parts.Last();

        var joint = target.gameObject.AddComponent<HingeJoint2D>();
        joint.connectedBody = part;
        joint.limits = new JointAngleLimits2D
        {
            max = m_jointLimit,
            min = -m_jointLimit,
        };

        var targetToPlayerDir = (PlayerMovement.Instance.transform.position - target.transform.position).normalized;
        part.transform.position = target.transform.position + (targetToPlayerDir * m_partLength / 2);

        m_parts.Add(part.transform);
        return part;
    }

    private Transform CreateFirstPart()
    {
        var part = CreatePart();
        part.bodyType = RigidbodyType2D.Kinematic;
        part.transform.position = m_attached.position;
        return part.transform;
    }

    private Rigidbody2D CreatePart()
    {
        var part = new GameObject("Part");
        part.transform.SetParent(m_currentRope);
        part.gameObject.layer = 10;
        part.AddComponent<RopeNode>();

        var partCollider = part.AddComponent<CircleCollider2D>();
        partCollider.radius = m_partThickness / 2;

        var rb = part.AddComponent<Rigidbody2D>();
        rb.mass = m_partMass;

        return rb;
    }
}
