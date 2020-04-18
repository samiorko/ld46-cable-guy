using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverText : MonoBehaviour
{

    [TextArea(1, 5)]
    public string m_text;

    public GameObject m_obj;
    public Vector2 m_textOffset;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(m_obj);
        if (!other.CompareTag("Player")) return;

        m_obj = Instantiate(GameManager.Instance.HoverTextPrefab);
        m_obj.GetComponentInChildren<Text>().text = m_text;

        m_obj.transform.position = (Vector2) transform.position + m_textOffset;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Destroy(m_obj);
    }
}
