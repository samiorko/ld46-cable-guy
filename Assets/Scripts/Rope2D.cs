using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope2D : MonoBehaviour
{

    public Rigidbody2D m_start;
    public Rigidbody2D m_end;

    public int m_partCount;
    public float m_partMass;

    public Sprite m_sprite;
    public bool m_pushRight;

    // Start is called before the first frame update
    private void Start()
    {
        var partVector = (m_end.position - m_start.position) / m_partCount;
        var colliderSize = partVector.magnitude;
        var parts = new LinkedList<GameObject>();
        parts.AddFirst(m_start.gameObject);

        for (var i = 1; i < m_partCount; i++)
        {
            var startPos = m_start.position + (partVector * i);

            var part = new GameObject();
            var rb = part.AddComponent<Rigidbody2D>();
            var addedCollider = part.AddComponent<CapsuleCollider2D>();
            part.transform.SetParent(transform);
            part.AddComponent<SpriteRenderer>().sprite = m_sprite;

            rb.mass = m_partMass;
            part.transform.position = startPos;
            addedCollider.size = new Vector2(colliderSize, .1f);
            addedCollider.direction = CapsuleDirection2D.Horizontal;

            AddRopeJoint<HingeJoint2D>(part, parts.Last.Value);
            parts.AddLast(part);
        }

        AddRopeJoint<HingeJoint2D>(m_end.gameObject, parts.Last.Value);
        parts.AddLast(m_end.gameObject);
    }

    private static void AddRopeJoint<T>(GameObject first, GameObject second) where T: Joint2D
    {
        var joint = first.AddComponent<T>();
        joint.connectedBody = second.GetComponent<Rigidbody2D>();
    }

    private void OnValidate()
    {
        if (!m_pushRight) return;

        m_pushRight = false;
        m_end.AddForce(Vector2.right * 100f, ForceMode2D.Impulse);
    }
}
