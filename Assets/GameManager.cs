using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")] 
    public GameObject HoverTextPrefab;

    private void Awake()
    {
        Instance = this;
    }
}
