using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool m_winningDoor;

    private bool m_opening;

    public void Open()
    {
        if (m_opening) return;
        StartCoroutine(nameof(OpenRoutine));
    }

    public IEnumerator OpenRoutine()
    {
        m_opening = true;
        var source = GetComponent<AudioSource>();
        source.Play();
        source.loop = false;

        while (source.isPlaying)
        {
            yield return null;
        }

        GetComponentInChildren<Collider2D>().gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_winningDoor && !m_opening)
        {
            if (SceneManager.Instance.State == SceneManager.SceneState.Winning)
            {
                this.Open();
            }
        }
    }
}
