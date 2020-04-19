using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public enum MovementMode
    {
        Normal,
        Ladder,
    }

    public MovementMode Mode { get; private set; } = MovementMode.Normal;

    public static PlayerMovement Instance { get; private set; }
    public float FeetHeight => m_feetAnchor.position.y;

    public Transform m_feetAnchor;

    public Transform[] m_feetAnchors;

    public bool IsGrounded;
    public float m_movementSpeed;
    public float m_ladderSpeed;

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
    private Ladder m_attachedLadder;

    private Rigidbody2D m_rb;
    public AudioClip m_jumpClip;

    public bool MovementAllowed => AllowMovementInState(SceneManager.Instance.State);


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

   
    private void Update()
    {
        if (!MovementAllowed) return;

        if (Mode == MovementMode.Normal)
        {
            HandleNormalMovement();
        }
        else if (Mode == MovementMode.Ladder)
        {
            m_jumping = false;
            IsGrounded = false;
            HandleLadderMovement();
        }
    }

    public void Exploded(Vector2 position)
    {
        var forceDir = (Vector2) transform.position - position;
        var forceAmount = Mathf.Sqrt(forceDir.magnitude);

        var force = forceDir.normalized * forceAmount * .1f;

        m_rb.bodyType = RigidbodyType2D.Dynamic;
        m_rb.freezeRotation = false;
        m_rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void HandleNormalMovement()
    {
        IsGrounded = m_feetAnchors.Any(x => Physics2D.OverlapCircle(x.position, .1f, m_groundLayers));

        var horizontal = Input.GetAxis("Horizontal") * m_movementSpeed;
        m_movement =  horizontal * Vector2.right;

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

        if (Mathf.Abs(Input.GetAxis("Vertical")) > .1f)
        {
            var ladders = Physics2D.OverlapCircle(transform.position, .2f, 1 << 12);
            if (ladders != null && ladders.gameObject != null)
            {
                var ladder = ladders.GetComponent<Ladder>();
                var isTop = ladder.IsTopEnd(transform.position);

                if (isTop && Input.GetAxis("Vertical") > .1f)
                {
                    AttachToLadder(ladder);
                } 
                else if (!isTop && Input.GetAxis("Vertical") < -.1f)
                {
                    AttachToLadder(ladder);
                }
            }
        }
    }

    private bool AllowMovementInState(SceneManager.SceneState state)
    {
        switch (state)
        {
            case SceneManager.SceneState.Running:
            case SceneManager.SceneState.Winning:
                return true;
            default:
                return false;
        }
    }


    private void AttachToLadder(Ladder ladder)
    {
        m_rb.bodyType = RigidbodyType2D.Kinematic;
        m_gravity = Vector2.zero;
        m_attachedLadder = ladder;
        var attachmentPoint = m_attachedLadder.GetAttachingPosition(transform.position);
        transform.position = attachmentPoint;
        Mode = MovementMode.Ladder;
    }

    private void DetachFromLadder()
    {
        m_rb.bodyType = RigidbodyType2D.Dynamic;
        m_attachedLadder = null;
        Mode = MovementMode.Normal;
    }

    private void HandleLadderMovement()
    {

        if (transform.position.y > m_attachedLadder.HighestPoint)
        {
            DetachFromLadder();
        } else if (m_feetAnchor.transform.position.y < m_attachedLadder.LowestPoint)
        {
            DetachFromLadder();
        }

        var movement = Input.GetAxis("Vertical") * m_ladderSpeed * Vector2.up;
        m_movement = movement;
    }

    private void FixedUpdate()
    {
        if (!m_jumping && !IsGrounded && Mode == MovementMode.Normal)
        {
            if (m_gravity == Vector2.zero)
            {
                m_gravity = Physics2D.gravity;
            }
            m_gravity += Physics2D.gravity * Time.fixedDeltaTime;
        }
        else
        {
            m_gravity = Vector2.zero;
        }

        var newPosition = (Vector2)transform.position + m_movement + (m_gravity * Time.fixedDeltaTime);

        if (m_jumping)
        {
            newPosition.y = m_currentJumpHeight;
        }

        m_rb.MovePosition(newPosition);
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

        var source = GetComponent<AudioSource>();
        source.PlayOneShot(m_jumpClip);

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
}
