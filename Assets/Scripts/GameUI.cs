using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    public Text m_lengthText;
    public Transform m_serversContainer;
    public GameObject m_serverStatusPrefab;
    public Text m_usersText;
    public Text m_overloadingText;
    public Text m_finalWarningText;

    private void Start()
    {
        Refresh();
        m_overloadingText.text = "Server overloading in ...";
        Invoke(nameof(StartUpdatingLoadingText), 2f);
        m_finalWarningText.gameObject.SetActive(false);
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

    private void StartUpdatingLoadingText()
    {
        StartCoroutine(nameof(UpdateOverloadingText));
    }

    private IEnumerator UpdateOverloadingText()
    {
        while (true)
        {
            var interval = 1f;
            if (SceneManager.Instance.CalculateOverloadTime(out var overLoadTime))
            {
                if (overLoadTime.TotalSeconds < 15)
                {
                    interval = .25f; // Increase check frequency when nearing the end
                }

                var seconds = (int) overLoadTime.TotalSeconds - 1;
                m_overloadingText.text = $"Server overloading in {seconds} seconds!";
                if (seconds <= 10)
                {
                    m_finalWarningText.gameObject.SetActive(true);
                    m_finalWarningText.text = $"{seconds}";
                    m_finalWarningText.fontSize = 50 + (10 - seconds) * 10;
                }
                else
                {
                    m_finalWarningText.gameObject.SetActive(false);
                }
            }
            else
            {
                m_finalWarningText.gameObject.SetActive(false);
                m_overloadingText.text = "Servers can handle it!";
            }

            yield return new WaitForSeconds(interval);
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
