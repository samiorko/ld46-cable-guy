using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNode : MonoBehaviour
{

    private Rigidbody2D m_rb;
    private int m_zeroVelocityFrames;

    private bool didShout;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (m_rb.velocity == Vector2.zero)
        {
            m_zeroVelocityFrames++;
        }
        else
        {
            m_zeroVelocityFrames = 0;
        }

        if (m_zeroVelocityFrames > 1000)
        {
            m_rb.bodyType = RigidbodyType2D.Static;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }
}
