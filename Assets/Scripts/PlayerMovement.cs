using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    public float FeetHeight => m_feetAnchor.position.y;

    public Transform m_feetAnchor;

    public Transform[] m_feetAnchors;

    public bool IsGrounded;
    public float m_movementSpeed;

    public float m_jumpHeight;
    public AnimationCurve m_jumpCurve;
    public float m_jumpDuration;

    private Vector2 m_movement;
    private Vector2 m_gravity;

    public LayerMask m_groundLayers;

    public float m_castDistance;

    public Vector2 Velocity => m_movement + m_gravity;

    private bool m_preventJumpingUntilAirborne;
    private bool m_jumping;

    private float m_currentJumpHeight;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        IsGrounded = m_feetAnchors.Any(x => Physics2D.OverlapCircle(x.position, .1f, m_groundLayers));
        var movement = GetMovement();
        m_movement = movement;
        var hitInfo = Physics2D.Raycast(transform.position, m_movement.x > 0f ? Vector2.right : Vector2.left, m_castDistance, m_groundLayers);
        if (hitInfo.collider != null)
        {
            m_movement = m_movement.normalized * hitInfo.distance;
        }

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
    }

    private void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody2D>();

        if (!m_jumping && !IsGrounded)
        {
            if (m_gravity == Vector2.zero)
            {
                m_gravity = Physics2D.gravity;
            }
            m_gravity += Physics2D.gravity * Time.fixedDeltaTime;
        }

        var newPosition = (Vector2)transform.position + m_movement + (m_gravity * Time.fixedDeltaTime);

        if (m_jumping)
        {
            newPosition.y = m_currentJumpHeight;
        }

        rb.MovePosition(newPosition);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
            var currentY = startHeight + m_jumpHeight * m_jumpCurve.Evaluate(t);
            m_currentJumpHeight = currentY;
            yield return null;
        }

        m_jumping = false;
        m_currentJumpHeight = 0f;
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
