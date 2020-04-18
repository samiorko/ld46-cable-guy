using UnityEngine;
using UnityEngine.UI;

public class HoverText : MonoBehaviour
{

    [TextArea(1, 5)]
    public string m_text = "";

    public GameObject m_obj;
    public Vector2 m_textOffset;

    private string m_textOverride;
    public bool ShowOnPlayer { get; set; }

    private void Update()
    {
        if (ShowOnPlayer && m_obj != null)
        {
            m_obj.transform.position = (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position + (Vector2.up * .5f);
        }
    }

    public void Show()
    {
        Destroy(m_obj);
        m_obj = Instantiate(GameManager.Instance.HoverTextPrefab);

        SetText(m_textOverride ?? m_text, false);
        
        m_obj.transform.position = (Vector2)transform.position + m_textOffset;
    }

    public void Hide()
    {
        Destroy(m_obj);
    }

    public void SetText(string newText) => SetText(newText, true);

    public void SetText(string newText, bool persist)
    {
        newText = GameManager.Instance.ParseText(newText);
        m_obj.GetComponentInChildren<Text>().text = newText;
        if (persist)
        {
            m_textOverride = newText;
        }
    }
}
