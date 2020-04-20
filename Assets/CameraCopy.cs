using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCopy : MonoBehaviour
{
    private Camera m_camera;

    private void Start()
    {
        m_camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        m_camera.orthographicSize = Camera.main.orthographicSize;
        m_camera.nearClipPlane = Camera.main.nearClipPlane;
        m_camera.farClipPlane = Camera.main.farClipPlane;
    }
}
