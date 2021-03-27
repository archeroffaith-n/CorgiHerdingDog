using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public GameObject corgi;
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    public float R;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Update()
    {
        yaw = yaw + speedH * Input.GetAxis("Mouse X");
        pitch = Mathf.Clamp(pitch - speedV * Input.GetAxis("Mouse Y"), 0.0f, 90.0f);
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        float phi = -Mathf.Deg2Rad * yaw;
        float theta = Mathf.Deg2Rad * pitch;
        transform.position = new Vector3(R * Mathf.Cos(theta) * Mathf.Sin(phi) , R * Mathf.Sin(theta), -R * Mathf.Cos(theta) * Mathf.Cos(phi)) + corgi.transform.position;
    }
}
