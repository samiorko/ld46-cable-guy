using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    public Text m_lengthText;
    public Transform m_serversContainer;
    public GameObject m_serverStatusPrefab;

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (RopeReel.Instance.Active)
        {
            m_lengthText.text = $"{(int) RopeReel.Instance.CurrentRopeLength}m";
        }
        else
        {
            m_lengthText.text = "";
        }
    }

    public void Refresh()
    {
        foreach (Transform obj in m_serversContainer)
        {
            Destroy(obj.gameObject);
        }

        foreach (var rack in SceneManager.Instance.Racks)
        {
            var obj = Instantiate(m_serverStatusPrefab).GetComponent<ServerStateBar>();
            obj.transform.SetParent(m_serversContainer);

            obj.m_rack = rack;
        }
    }
}
