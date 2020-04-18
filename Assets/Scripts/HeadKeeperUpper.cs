using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadKeeperUpper : MonoBehaviour
{
    public ForceMode2D m_mode;
    public Vector2 m_force;
    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().AddForce(m_force, m_mode);
        GetComponent<Joint2D>().connectedBody.AddForce(-m_force, m_mode);
    }
}
