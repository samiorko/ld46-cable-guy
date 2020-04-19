using UnityEngine;

public class LevelTutorial : MonoBehaviour
{
    public float m_winThreshold;
    void Update()
    {
        var rack = SceneManager.Instance.Racks[0];
        if(SceneManager.Instance.State == SceneManager.SceneState.Running && rack.Load < m_winThreshold && SceneManager.Instance.LinkedRacks(rack).Count == 1)
        {
            SceneManager.Instance.SetWinningState();
        }
    }
}
