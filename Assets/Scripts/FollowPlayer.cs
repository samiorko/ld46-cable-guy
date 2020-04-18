using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform m_player;
    public float m_smoothTime;
    public float m_maxSpeed;
    public float m_cameraDistance;
    private Vector2 m_currentVelocity;

    public void LateUpdate()
    {
        var target = m_player.position;

        transform.position =
            Vector2.SmoothDamp(transform.position, target, ref m_currentVelocity, m_smoothTime, m_maxSpeed);

        transform.position += Vector3.back * m_cameraDistance;

    }
}
