using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroBanner : MonoBehaviour
{
    public static IntroBanner Instance { get; private set; }

    public RectTransform m_background;

    public float m_targetHeight;
    public float m_targetWidth;
    public float m_durationWidth;
    public float m_durationHeight;

    public float Duration => Mathf.Max(m_durationHeight, m_durationWidth) + m_keepOpenDuration;

    public Text m_headerTextComponent;
    public Text m_subHeaderTextComponent;

    private float m_keepOpenDuration;

    private void Awake()
    {
        Instance = this;    
    }

    //private void OnEnable()
    //{
    //    Show(m_header, m_subHeader);
    //}

    public void Show(string header, string subHeader, float duration = 5f)
    {
        m_keepOpenDuration = duration;
        m_headerTextComponent.text = header;
        m_subHeaderTextComponent.text = subHeader;

        m_background.transform.localScale = Vector2.zero;
        m_background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_targetHeight);
        m_background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_targetWidth);

        m_background.gameObject.SetActive(true);

        StartCoroutine(nameof(Open));
    }

    public IEnumerator Open()
    {
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

        if (Mathf.Approximately(m_keepOpenDuration, float.MaxValue)) yield break;

        yield return new WaitForSeconds(m_keepOpenDuration);
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

        m_background.gameObject.SetActive(false);
    }
}
