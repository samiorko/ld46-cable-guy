using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")] 
    public GameObject HoverTextPrefab;

    public GameObject Player { get; private set; }
    public int m_levelCount;


    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.StartsWith("Level_") && Input.GetKeyDown(KeyCode.R))
        {
            LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public string ParseText(string text)
    {
        text = text.Replace("%CABLE_M%", (RopeReel.Instance.CurrentRopeLength).ToString());

        return text;
    }

    public void LevelCompleted()
    {
        LoadScene(GetNextSceneName());
    }

    public void RestartLevel()
    {

    }

    private string GetNextSceneName()
    {
        var currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentLevel.StartsWith("Level_"))
        {
            var currentLevelNumber = int.Parse(currentLevel.Replace("Level_", ""));
            return currentLevelNumber == m_levelCount ? "Final" : $"Level_{currentLevelNumber + 1}";
        }

        if (currentLevel.Equals("Tutorial")) return "Level_1";

        throw new Exception("Could not determine next scene");
    }

    private void LoadScene(string name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }
}
