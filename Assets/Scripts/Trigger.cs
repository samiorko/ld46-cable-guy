using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent m_onPlayerEnter;
    public UnityEvent m_onPlayerExit;

    public bool PlayerInside { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerInside = true;
        m_onPlayerEnter?.Invoke();;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerInside = false;
        m_onPlayerExit?.Invoke(); ;
    }
}
