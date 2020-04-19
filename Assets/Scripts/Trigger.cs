using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent m_onPlayerEnter;
    public UnityEvent m_onPlayerExit;

    public UnityEvent m_onPlayerEnterFirstTime;
    public UnityEvent m_onPlayerExitFirstTime;

    private bool m_didEnter;

    public bool PlayerInside { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerInside = true;

        if (!m_didEnter)
        {
            m_onPlayerEnterFirstTime?.Invoke();;
        }

        m_onPlayerEnter?.Invoke();;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerInside = false;
        if (!m_didEnter)
        {
            m_didEnter = true;
            m_onPlayerExitFirstTime?.Invoke();
        }
        m_onPlayerExit?.Invoke(); ;
    }
}
