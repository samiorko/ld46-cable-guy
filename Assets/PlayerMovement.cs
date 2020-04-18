using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    public Transform[] m_feetAnchors;

    public bool IsGrounded;
    public float m_movementSpeed;

    public float m_jumpHeight;
    public AnimationCurve m_jumpCurve;
    public float m_jumpDuration;

    private Vector2 m_movement;
    private Vector2 m_gravity;

    public Vector2 Velocity => m_movement + m_gravity;

    private bool m_preventJumpingUntilAirborne;
    private bool m_jumping;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        IsGrounded = m_feetAnchors.Any(x => Physics2D.OverlapCircle(x.position, .1f, 1 << 8));
        var movement = GetMovement();
        m_movement = movement;

        if (IsGrounded)
        {
            if (!m_preventJumpingUntilAirborne)
            {
                StopCoroutine(nameof(Jump));
                m_jumping = false;
                if (Input.GetButtonDown("Jump"))
                {
                    StartCoroutine(nameof(Jump));
                }
            }
            m_gravity = Vector2.zero;
        }

        transform.Translate(m_movement * Time.deltaTime);

        if (!m_jumping && !IsGrounded)
        {
            if (m_gravity == Vector2.zero)
            {
                m_gravity = Physics2D.gravity;
            }
            m_gravity += Physics2D.gravity * Time.deltaTime;
            transform.Translate(m_gravity * Time.deltaTime, Space.World);
        }
    }

    private IEnumerator Jump()
    {
        m_jumping = true;
        m_preventJumpingUntilAirborne = true;
        var elapsedTime = 0f;
        var startHeight = transform.position.y;

        while (elapsedTime < m_jumpDuration)
        {
            if (m_preventJumpingUntilAirborne && !IsGrounded)
            {
                m_preventJumpingUntilAirborne = false;
            }
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / m_jumpDuration;
            var currentY= startHeight + m_jumpHeight * m_jumpCurve.Evaluate(t);
            transform.position = new Vector2(transform.position.x, currentY);
            yield return null;
        }

        m_jumping = false;
        m_preventJumpingUntilAirborne = false; // fail safe to make sure it doesn't get stuck
    }

    private Vector2 GetMovement()
    {
        var horizontal = Input.GetAxis("Horizontal") * m_movementSpeed;
        return horizontal * Vector2.right;
        //var jump = Input.GetButtonDown("Jump");

        //float maxMovementSpeedAdjustment;

        //if (Mathf.Approximately(Mathf.Sign(m_velocity.x), math.sign(horizontal)))
        //{
        //    maxMovementSpeedAdjustment = m_movementSpeed - m_velocity.x;
        //}
        //else
        //{
        //    maxMovementSpeedAdjustment = m_movementSpeed + m_velocity.x;
        //}

        //var movement = Mathf.Clamp(horizontal * m_movementSpeed, 0f, maxMovementSpeedAdjustment) * Vector2.right;

        //movement += Mathf.Clamp(horizontal * m_movementSpeed, m_velocity.x + m_movementSpeed, m_velocity.x - m_movementSpeed) * Vector2.right;

        //return movement;
    }
}
