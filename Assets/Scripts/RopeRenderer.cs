using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    public IReadOnlyList<Transform> Points => m_points;
    private List<Transform> m_points;
    public float m_lineWidth;
    private LineRenderer m_lineRenderer;

    private static Material RopeMaterial(Color color, Shader shader) => _material.ContainsKey(color) ? _material[color] : CreateMaterial(color, shader);
    private static readonly Dictionary<Color, Material> _material = new Dictionary<Color, Material>();

    private void Start()
    {
        gameObject.tag = "Rope";
    }

    // Start is called before the first frame update
    public void Setup(List<Transform> pointsList, float thickness, Color color, Shader shader)
    {
        m_lineWidth = thickness;
        m_lineRenderer = gameObject.AddComponent<LineRenderer>();
        m_lineRenderer.material = RopeMaterial(color, shader);

        m_points = pointsList;
    }

    private static Material CreateMaterial(Color color, Shader shader)
    {
        var material = new Material(shader) {color = color, name = $"Rope {color}"};

        _material.Add(color, material);
        return material;
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
