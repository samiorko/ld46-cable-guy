using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    public Text m_lengthText;
    public Transform m_serversContainer;
    public GameObject m_serverStatusPrefab;
    public Text m_usersText;
    public Text m_overloadingText;

    private void Start()
    {
        Refresh();
        m_overloadingText.text = "Server overloading in...";
        StartCoroutine(nameof(UpdateOverloadingText));
    }

    private void Update()
    {
        m_usersText.text = $"{SceneManager.Instance.CurrentUsers} users online";
        if (RopeReel.Instance.Active)
        {
            m_lengthText.text = $"{(int) RopeReel.Instance.CurrentRopeLength}m";
        }
        else
        {
            m_lengthText.text = "";
        }
    }

    private IEnumerator UpdateOverloadingText()
    {
        while (true)
        {
            if (SceneManager.Instance.CalculateOverloadTime(out var overLoadTime))
            {
                m_overloadingText.text = $"Server overloading in {overLoadTime.TotalSeconds} seconds";
            }
            else
            {
                m_overloadingText.text = "Servers can handle it!";
            }

            yield return new WaitForSeconds(1f);
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
