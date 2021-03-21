using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corgiControl : MonoBehaviour
{private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public GameObject camera;


    private void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
    }

    void Update()
    {
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(x, 0, z).normalized;
        move = Quaternion.AngleAxis(camera.transform.rotation.eulerAngles.y, Vector3.up) * move;

        float targetAngle = Mathf.Atan2(x, z) * Mathf.Rad2Deg + camera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        controller.Move(move * Time.deltaTime * playerSpeed);

        
    }
}
