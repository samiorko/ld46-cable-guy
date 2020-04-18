using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    private List<Transform> m_points;
    public float m_lineWidth;
    private LineRenderer m_lineRenderer;

    private static Material RopeMaterial => _material ?? CreateMaterial();
    private static Material _material;

    // Start is called before the first frame update
    public void Setup(List<Transform> pointsList, float thickness)
    {
        m_lineWidth = thickness;
        m_lineRenderer = gameObject.AddComponent<LineRenderer>();
        m_lineRenderer.material = RopeMaterial;
        m_points = pointsList;
    }

    private static Material CreateMaterial()
    {
        _material = new Material(Shader.Find("Specular"))
        {
            color = Color.black
        };

        return _material;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_points == null)
        {
            return;
        }
        m_lineRenderer.startWidth = m_lineWidth;
        m_lineRenderer.endWidth = m_lineWidth;
        m_lineRenderer.positionCount = m_points.Count;
        m_lineRenderer.SetPositions(m_points.Select(x => x.position).ToArray());
    }
}
