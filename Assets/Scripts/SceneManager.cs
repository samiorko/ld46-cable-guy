using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public enum SceneState
    {
        Running,
        Losing,
        Lost,
        Winning,
        Won,
    }

    public static SceneManager Instance { get; private set; }

    public SceneState State { get; private set; } = SceneState.Running;

    private SceneState m_previousState;

    public float Duration => m_duration;

    public List<Rack> Racks;
    private Dictionary<Rack, List<Rack>> m_linkedRacks;

    public float m_maxUsersPerRack;

    public AnimationCurve m_curve;

    public float m_duration;

    private float m_elapsed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    private void Start()
    {
        Racks = GameObject.FindGameObjectsWithTag("Server").Select(x => x.GetComponent<Rack>()).OrderBy(x => x.m_order).ToList();
        m_linkedRacks = Racks.ToDictionary(x => x, _ => new List<Rack>());
    }

    private void Update()
    {
        m_elapsed += Time.deltaTime;
        var t = m_elapsed / m_duration;

        var targetLoad = m_curve.Evaluate(t) * Racks.Count;
        AddLoad(targetLoad);

        DistributeLoads();
    }

    private void LateUpdate()
    {
        m_previousState = State;
    }

    private void AddLoad(float load)
    {
        Racks[0].Load = load;
    }

    private void DistributeLoads()
    {
        var rackGrid = FindAllLinkedRacks(Racks[0], new HashSet<Rack>());
        var averageLoad = Racks[0].Load / rackGrid.Count;
        foreach (var rack in rackGrid)
        {
            rack.Load = averageLoad;
        }
    }

    private ISet<Rack> FindAllLinkedRacks(Rack from, ISet<Rack> foundRacks)
    {
        foundRacks.Add(from);

        foreach (var rack in m_linkedRacks[from].Except(foundRacks))
        {
            FindAllLinkedRacks(rack, foundRacks);
        }

        return foundRacks;
    }

    public List<Rack> LinkedRacks(Rack rack)
    {
        return m_linkedRacks[rack];
    }

    public void LinkRacks(Rack first, Rack second)
    {
        m_linkedRacks[first].Add(second);
        m_linkedRacks[second].Add(first);
    }
}
