using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropCollider : MonoBehaviour
{
    private float m_topBound;
    private Collider2D m_collider;

    void Start()
    {
        m_collider = GetComponent<Collider2D>();
        m_topBound = m_collider.bounds.max.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") < -.1f)
        {
            m_collider.enabled = false;
        }
        else
        {
            m_collider.enabled = m_topBound < PlayerMovement.Instance.FeetHeight;
        }
        //gameObject.layer = m_collider.enabled ? 9 : 0;
    }
}
