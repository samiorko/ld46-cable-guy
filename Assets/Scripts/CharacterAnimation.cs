using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator m_animator;
    private SpriteRenderer m_renderer;

    public float m_walkThreshold;
    public float m_flipThreshold;

    private bool m_flip;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var movement = Input.GetAxis("Horizontal");
        if (movement > m_flipThreshold)
        {
            m_flip = false;
        }
        else if (movement < -m_flipThreshold)
        {
            m_flip = true;
        }

        m_animator.SetFloat("walkSpeed", Mathf.Abs(movement));
        m_animator.SetBool("flip", m_flip);

        //m_animator.speed = Mathf.Abs(movement);
        m_renderer.flipX = m_flip;
    }
}
