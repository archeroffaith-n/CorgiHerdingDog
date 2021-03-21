using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public GameObject corgi;
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    public float R;
    public float h;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        float phi = -Mathf.Deg2Rad * yaw;
        transform.position = new Vector3(R * Mathf.Sin(phi), h, -R * Mathf.Cos(phi)) + corgi.transform.position;
    }
}
