using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public Camera m_camera;

    void Start()
    {
        
    }

    void Update()
    {
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
        transform.position = ray.origin + ray.direction * Mathf.Abs(ray.origin.z / ray.direction.z);
    }
}
