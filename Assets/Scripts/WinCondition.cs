using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public bool m_allServersConnected;

    private void Update()
    {
        if (SceneManager.Instance.State != SceneManager.SceneState.Running) return;

        if (m_allServersConnected)
        {
            if (SceneManager.Instance.FindAllLinkedRacks(SceneManager.Instance.Racks[0]).Count !=
                SceneManager.Instance.Racks.Count)
            {
                return;
            }
        }


        SceneManager.Instance.SetWinningState();
    }
}
