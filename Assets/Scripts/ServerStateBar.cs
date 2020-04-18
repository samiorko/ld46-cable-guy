using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerStateBar : MonoBehaviour
{
    public RectTransform m_fill;
    public Gradient m_fillGradient;
    public Rack m_rack;

    void Update()
    { 
        m_fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30f * m_rack.Load);
        m_fill.GetComponent<Image>().color = m_fillGradient.Evaluate(m_rack.Load);
    }
}
