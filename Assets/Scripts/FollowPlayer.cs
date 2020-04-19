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

    public Transform m_boundsMin;
    public Transform m_boundsMax;

    public void LateUpdate()
    {
        var target = ClampVector(m_player.position, m_boundsMin.position, m_boundsMax.position);

        transform.position =
            Vector2.SmoothDamp(transform.position, target, ref m_currentVelocity, m_smoothTime, m_maxSpeed);

        transform.position += Vector3.back * m_cameraDistance;

    }

    private Vector2 ClampVector(Vector2 value, Vector2 min, Vector2 max)
    {
        var x = value.x;
        var y = value.y;

        if (x < min.x) x = min.x;
        if (x > max.x) x = max.x;
        if (y < min.y) y = min.y;
        if (y > max.y) y = max.y;

        return new Vector2(x, y);

    }
}
