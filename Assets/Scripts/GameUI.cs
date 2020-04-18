using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    public Text m_lengthText;
    void Update()
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
}
