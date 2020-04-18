using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Dictionary<string, string> Dictionary { get; } = new Dictionary<string, string>
    {
        {"CABLE_M", "0"},
    };

    [Header("UI")] 
    public GameObject HoverTextPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public string ParseText(string text)
    {
        return text.Replace("%CABLE_M%", ((int) RopeReel.Instance.CurrentRopeLength).ToString());
    }
}
