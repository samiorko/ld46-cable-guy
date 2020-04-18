using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HoverText))]
public class Interact : MonoBehaviour
{
    private Trigger m_trigger;
    public bool m_singleUse;
    public UnityEvent m_onInteract;

    private int m_useCount;

    private void Start()
    {
        m_trigger = GetComponent<Trigger>();
    }

    private void Update()
    {
        if (!m_trigger.PlayerInside) return;
        if (m_singleUse && m_useCount != 0) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            m_useCount++;
            m_onInteract?.Invoke();
        }
    }
}
