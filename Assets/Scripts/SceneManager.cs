using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public enum SceneState
    {
        Prep,
        Running,
        Losing,
        Lost,
        Winning,
        Won,
    }

    public static SceneManager Instance { get; private set; }

    public string m_header;
    public string m_subHeader;

    public int CurrentUsers { get; private set; }

    public SceneState State
    {
        get => _state;
        private set => _state = value;
    }

    [SerializeField]
    private SceneState _state = SceneState.Prep;

    private SceneState m_previousState;
    private bool m_stateDidChange;

    public float Duration => m_duration;

    public List<Rack> Racks;
    private Dictionary<Rack, List<Rack>> m_linkedRacks;

    public float m_maxUsersPerRack;

    public AnimationCurve m_curve;

    public float m_duration;

    private float m_elapsed;

    [Range(0,1)]
    public float m_currentLoad;
    
    [Range(0,1)]
    public float m_currentT;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;

        Racks = GameObject.FindGameObjectsWithTag("Server").Select(x => x.GetComponent<Rack>()).OrderBy(x => x.m_order).ToList();
        m_linkedRacks = Racks.ToDictionary(x => x, _ => new List<Rack>());
    }

    private void Update()
    {
        if (State == SceneState.Running)
        {
            m_elapsed += Time.deltaTime;
            m_currentT = m_elapsed / m_duration;

            m_currentLoad = m_curve.Evaluate(m_currentT) * Racks.Count;
            AddLoad(m_currentLoad);
            DistributeLoads();
        }

        if (m_stateDidChange)
        {
            if (State == SceneState.Running)
            {
                IntroBanner.Instance.Show(m_header, m_subHeader);
            }

            if (State == SceneState.Winning)
            {
                var ropeTotalLength = (int) GameObject.FindGameObjectsWithTag("Rope")
                    .Select(x => (x.GetComponent<RopeRenderer>().Points.Count * RopeReel.Instance.m_partLength) / 2)
                    .Sum();

                if (ropeTotalLength < 100)
                {
                    IntroBanner.Instance.Show("Level complete!", $"You laid {ropeTotalLength} meters of cable.");
                } else if (ropeTotalLength < 500)
                {
                    IntroBanner.Instance.Show("Level complete!", $"You laid {ropeTotalLength} meters of cable. Nice!");
                } else if (ropeTotalLength < 1000)
                {
                    IntroBanner.Instance.Show("Level complete!", $"You laid {ropeTotalLength} meters of cable. That seems... excessive");
                } else if (ropeTotalLength < 1000)
                {
                    IntroBanner.Instance.Show("Level complete!", $"You laid {ropeTotalLength} meters of cable. That's crazy!");
                } else
                {
                    IntroBanner.Instance.Show("Level complete!", $"{ropeTotalLength} meters...");
                }
            }

            if (State == SceneState.Won)
            {
                GameManager.Instance.LevelCompleted();
            }
        }
    }

    private void LateUpdate()
    {
        m_stateDidChange = m_previousState != State;
        m_previousState = State;
    }

    public void SetWinningState()
    {
        State = SceneState.Winning;
    }

    public void SetWonState()
    {
        State = SceneState.Won;
    }

    public void SetLosingState()
    {
        State = SceneState.Losing;
    }

    public void SetLostState()
    {
        State = SceneState.Lost;
    }

    private void AddLoad(float load)
    {
        if (!Racks.Any()) return;
        Racks[0].Load = load;
        CurrentUsers = (int) (load * m_maxUsersPerRack);
        if (CurrentUsers < 0) CurrentUsers = 0;
    }

    private void DistributeLoads()
    {
        if (!Racks.Any()) return;
        var rackGrid = FindAllLinkedRacks(Racks[0], new HashSet<Rack>());
        var averageLoad = Racks[0].Load / rackGrid.Count;
        foreach (var rack in rackGrid)
        {
            rack.Load = averageLoad;
        }
    }

    public ISet<Rack> FindAllLinkedRacks(Rack from, ISet<Rack> foundRacks = null)
    {
        if (foundRacks == null)
        {
            foundRacks = new HashSet<Rack>();
        }

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

    public bool CalculateOverloadTime(out TimeSpan overloadTime)
    {
        // Figure out using the current curve when the server would overload. Do it by just simply trying out values until hit.
        var racksConnected = FindAllLinkedRacks(Racks[0]).Count;
        var currentCapacity = racksConnected / (float) Racks.Count;

        var remainingT = 1 - m_currentT;
        var step = 1f / m_duration; // get step equivalent of one second
        var steps = remainingT / step;

        for (var i = 1; i < steps; i++)
        {
            var evaluated = m_curve.Evaluate(m_currentT + step * i);
            if (Mathf.Approximately(evaluated, currentCapacity) || evaluated > currentCapacity)
            {
                overloadTime = TimeSpan.FromSeconds(m_duration * (step * i));
                return true;
            }
        }

        return false;
    }
}
