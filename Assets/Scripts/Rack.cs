using System.Collections;
using System.Linq;
using UnityEngine;

public class Rack : MonoBehaviour
{
    public SpriteRenderer[] m_lights;
    public bool[] m_lightStates;

    [Range(0f, 1f)]
    public float m_rackLoad;

    public Color m_offColor;
    public Color m_greenColor;
    public Color m_yellowColor;
    public Color m_redColor;

    public float m_okBlinkSpeed;
    public float m_dangerBlinkSpeed;

    private int m_currentLayer;

    private void Start()
    {
        m_lightStates = m_lights.Select(_ => true).ToArray();
        for (var i = 0; i < m_lights.Length; i++)
        {
            // use coroutines to blink lights independently
            StartCoroutine(nameof(LayerLightBlinker), i);
        }
    }

    private void Update()
    {
        m_currentLayer = Mathf.FloorToInt(m_lights.Length * m_rackLoad);
        for (var i = 0; i < m_lights.Length; i++)
        {
            m_lights[i].color = GetColorForLayer(i);
        }
    }

    private IEnumerator LayerLightBlinker(int layer)
    {
        while (true)
        {
            m_lightStates[layer] = !m_lightStates[layer];
            var layerT = GetLayerT(layer);
            var waitTime = Mathf.Lerp(m_okBlinkSpeed, m_dangerBlinkSpeed, layerT);
            if (m_currentLayer != layer)
            {
                waitTime += (waitTime * Random.value) / 2; // add some randomness to make the blinking less boring looking
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    private Color GetColorForLayer(int layer)
    {
        if (!m_lightStates[layer])
        {
            return m_offColor;
        }

        if (m_currentLayer != layer)
        {
            return m_currentLayer < layer ? m_greenColor : m_redColor;
        }

        return m_yellowColor;
    }

    private float GetLayerT(int layer)
    {
        // 0 = ok, 1 = bad
        if (m_currentLayer != layer)
        {
            return m_currentLayer < layer ? 0f : 1f;
        }

        var layerT = m_lights.Length * m_rackLoad % 1f;

        return layerT;
    }

}
