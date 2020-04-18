using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePickerUp : MonoBehaviour
{
    public List<Transform> Parts { get; set; }
    public Rack Origin { get; set; }

    private HoverText m_hoverText;

    private bool m_playerInside;

    public void Awake()
    {
        m_hoverText = new GameObject().AddComponent<HoverText>();
        m_hoverText.ShowOnPlayer = true;
        var triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = 2f;
    }

    private void Update()
    {
        if (m_playerInside && Input.GetKeyDown(KeyCode.Q))
        {
            PickUp();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        m_playerInside = true;

        if (RopeReel.Instance.Active) return;
        m_hoverText.Show();
        m_hoverText.SetText("[Press Q to pick up cable]");
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        m_playerInside = false;
        m_hoverText.Hide();
    }

    public void PickUp()
    {
        RopeReel.Instance.PickUp(this);
    }
}
