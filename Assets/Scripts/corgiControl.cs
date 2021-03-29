using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corgiControl : MonoBehaviour
{private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    public float turnSmoothTime = 0.1f;
    public float barkDistance = 10.0f;
    private LayerMask sheepLayerMask;
    float turnSmoothVelocity;
    public GameObject mainCamera;


    private void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
        sheepLayerMask = LayerMask.GetMask("Sheep");
    }

    void Update()
    {
        // Update desired direction for sheeps in radius
        if (Input.GetKeyDown(KeyCode.Space)) {
            Collider[] barkSheeps = Physics.OverlapSphere(transform.position, barkDistance, sheepLayerMask);
            foreach (var sheepCollider in barkSheeps)
            {
                sheepMovement sheepScript = sheepCollider.gameObject.GetComponent<sheepMovement>();
                sheepScript.desiredDirection = (sheepCollider.gameObject.transform.position - transform.position).normalized;
                sheepScript.decreaseTime = 0.0f;
            }
        }

        // Move
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(x, 0, z).normalized;
        move = Quaternion.AngleAxis(mainCamera.transform.rotation.eulerAngles.y, Vector3.up) * move;
        controller.Move((move - new Vector3(0, move.y, 0)) * Time.deltaTime * playerSpeed);

        // Fix height
        transform.position = transform.position - new Vector3(0, transform.position.y - 1.0f, 0);
    }

    void LateUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Rotate
        float targetAngle = Mathf.Atan2(x, z) * Mathf.Rad2Deg + mainCamera.transform.rotation.eulerAngles.y + 90;
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }
}
