using System;
using UnityEngine;
using UnityEngine.UI;

public class IntroLevel : MonoBehaviour
{
    public Text m_text;

    private void Start()
    {
        m_text.text = m_text.text.Replace("%CURRENT_DATE%", DateTime.Now.ToString("dd.MM.yyyy"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
        }
    }

}
