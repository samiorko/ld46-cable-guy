using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeReel : MonoBehaviour
{
    public static RopeReel Instance { get; private set; }
    public bool Active => m_attached != null;
    public float CurrentRopeLength => (m_partLength * (m_parts?.Count ?? 0)) / 2;

    public Rack m_attachOnStart;
    public Joint2D m_playerJoint;

    public float m_partLength;
    public float m_partMass;
    public float m_partThickness;
    public float m_jointLimit;

    private List<Transform> m_parts;

    private Rack m_attached;
    private Transform m_currentRope;

    [TextArea(1, 5)]
    public string m_hoverTextAttach;
    [TextArea(1, 5)]
    public string m_hoverTextCantAttachSame;
    [TextArea(1, 5)]
    public string m_hoverTextConnect;

    public Color[] m_cableColors;

    public Shader m_shader;

    private void Start()
    {
        Instance = this;
        if (m_attachOnStart != null)
        {
            Attach(m_attachOnStart);
        }
    }

    private void Update()
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

        if (Input.GetKeyDown(KeyCode.Q) && m_attached)
        {
            Detach(true);
        }
    }

    public bool Attach(Rack rack)
    {
        if (!m_attached)
        {
            m_parts = new List<Transform>();
            m_currentRope = new GameObject("Rope").transform;
            m_currentRope.transform.position = rack.m_attachTarget.position + (Vector3.back * .1f);
            var ropeRenderer = m_currentRope.gameObject.AddComponent<RopeRenderer>();
            var randomColor = m_cableColors[Random.Range(0, m_cableColors.Length)];
            ropeRenderer.Setup(m_parts, m_partThickness, randomColor, m_shader);
            m_attached = rack;

            m_parts.Add(CreateFirstPart());
            CreateLinkPart();
            return true;
        }
        
        if (rack == m_attached)
        {
            return false;
            // cannot attach to same
        }

        // Link!
        m_parts.Add(CreateLastPart(rack.m_attachTarget));
        SceneManager.Instance.LinkRacks(m_attached, rack);
        Detach(false);
        return true;
    }

    public void Detach(bool createPickup)
    {
        if (createPickup)
        {
            var pickupObject = new GameObject("Pickup").AddComponent<CablePickerUp>();
            pickupObject.transform.SetParent(m_parts.Last().transform);
            pickupObject.transform.localPosition = Vector2.zero;
            pickupObject.Origin = m_attached;
            pickupObject.Parts = m_parts;
        }

        m_attached = null;
        m_parts = new List<Transform>();
    }

    public string GetHoverText(Transform otherTransform)
    {
        if (!m_attached)
        {
            return m_hoverTextAttach;
        }

        return m_attached == otherTransform ? m_hoverTextCantAttachSame : m_hoverTextConnect;
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
        part.transform.position = m_attached.m_attachTarget.position;
        return part.transform;
    }

    private Transform CreateLastPart(Transform endingPosition)
    {
        var target = m_parts.Last();
        var part = CreatePart();
        part.bodyType = RigidbodyType2D.Kinematic;
        part.transform.position = endingPosition.position;

        var joint = target.gameObject.AddComponent<HingeJoint2D>();
        joint.connectedBody = part;
        joint.limits = new JointAngleLimits2D
        {
            max = m_jointLimit,
            min = -m_jointLimit,
        };

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

    public void PickUp(CablePickerUp cablePickerUp)
    {
        if (m_attached != null) return;

        m_attached = cablePickerUp.Origin;
        m_parts = cablePickerUp.Parts;
        CreateLinkPart();

        Destroy(cablePickerUp.gameObject);
    }
}
