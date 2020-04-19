using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroBanner : MonoBehaviour
{
    public RectTransform m_background;

    public float m_targetHeight;
    public float m_targetWidth;
    public float m_durationWidth;
    public float m_durationHeight;

    public string m_header;
    public string m_subHeader;

    public Text m_headerTextComponent;
    public Text m_subHeaderTextComponent;

    private void OnEnable()
    {
        Show(m_header, m_subHeader);
    }

    public void Show(string header, string subHeader)
    {
        m_header = header;
        m_subHeader = subHeader;

        m_headerTextComponent.text = header;
        m_subHeaderTextComponent.text = subHeader;

        m_background.transform.localScale = Vector2.zero;
        m_background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_targetHeight);
        m_background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_targetWidth);

        StartCoroutine(nameof(Open));
    }

    public IEnumerator Open()
    {

        yield return new WaitForSeconds(2f);
        var elapsed = 0f;
        var tWidth = 0f;
        var tHeight = 0f;

        while (tWidth < 1f || tHeight < 1f)
        {
            elapsed += Time.deltaTime;
            tHeight = Mathf.Clamp01(elapsed / m_durationHeight);
            tWidth = Mathf.Clamp01(elapsed / m_durationWidth);

            m_background.transform.localScale = new Vector2(tWidth, tHeight);
            yield return null;
        }

        yield return  new WaitForSeconds(5f);
        yield return StartCoroutine(nameof(Close));
    }

    public IEnumerator Close()
    {
        var elapsed = 0f;
        var tWidth = 0f;
        var tHeight = 0f;

        while (tWidth < 1f || tHeight < 1f)
        {
            elapsed += Time.deltaTime;
            tHeight = Mathf.Clamp01(elapsed / m_durationHeight);
            tWidth = Mathf.Clamp01(elapsed / m_durationWidth);

            m_background.transform.localScale = new Vector2(1f - tWidth,  1f - tHeight);
            yield return null;
        }
    }
}
