using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rack : MonoBehaviour
{
    public SpriteRenderer[] m_lights;
    public bool[] m_lightStates;

    public float Load
    {
        get => m_rackLoad;
        set => m_rackLoad = value;
    }

    [Range(0f, 1f)]
    public float m_rackLoad;

    public int m_order;

    public Color m_offColor;
    public Color m_greenColor;
    public Color m_yellowColor;
    public Color m_redColor;

    public Transform m_attachTarget;
    public float m_okBlinkSpeed;
    public float m_dangerBlinkSpeed;

    private int m_currentLayer;

    public ParticleSystem m_smokeParticles;

    [Range(0f, 1f)]
    public float m_shakeT;

    public float m_shakeAmount;

    public float m_shakeInterval;

    private Vector2 m_originalPosition;
    private float m_nextShake;
    public AudioClip m_attachClip;

    public GameObject m_explosionParent;
    private void Start()
    {
        m_attachTarget.transform.SetParent(null);
        m_originalPosition = transform.position;
        m_lightStates = m_lights.Select(_ => true).ToArray();
        for (var i = 0; i < m_lights.Length; i++)
        {
            // use coroutines to blink lights independently
            StartCoroutine(nameof(LayerLightBlinker), i);
        }
    }

    public void Explode()
    {

    }

    private void Update()
    {
        var emissions = m_smokeParticles.emission;
        m_currentLayer = Mathf.FloorToInt(m_lights.Length * m_rackLoad);
        for (var i = 0; i < m_lights.Length; i++)
        {
            m_lights[i].color = GetColorForLayer(i);
        }

        if (m_currentLayer > 1)
        {
            emissions.enabled = true;
            var emissionT = (m_currentLayer - 1) / (float) (m_lights.Length - 1);
            emissions.rateOverTimeMultiplier = emissionT * 10;
            m_shakeT = emissionT * emissionT;
        }
        else
        {
            emissions.enabled = false;
            m_shakeT = 0;
        }

        Shake();
    }

    public void AttachToPlayer()
    {
        if (RopeReel.Instance.Attach(this))
        {
            GetComponent<AudioSource>().PlayOneShot(m_attachClip);
        }
    }

    private void Shake()
    {
        if (m_nextShake > 0f)
        {
            m_nextShake -= Time.deltaTime;
            return;
        }

        m_nextShake = m_shakeInterval;
        transform.position = m_originalPosition + Random.insideUnitCircle * m_shakeAmount * m_shakeT;
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

    public void UpdateHoverText()
    {
        GetComponentInChildren<HoverText>().SetText(RopeReel.Instance.GetHoverText(m_attachTarget));
    }

}
