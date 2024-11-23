using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corgi : MonoBehaviour
{
    // Variable of state
    private Vector3 playerVelocity = Vector3.zero;
    private bool isGrounded;
    
    // Getters of state
    public Vector3 getVelocity() { return playerVelocity; }
    public bool getIsGrounded() { return isGrounded; }

    // Tunable parameters
    public float maxSpeed = 12.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    public float timeSmoothTurn = 0.1f;
    public float barkDistance = 10.0f;
    public float acceleration = 2.0f;
    public float deceleration = 4.0f;
    
    // Inner data
    private CharacterController controller;
    private LayerMask sheepLayerMask;
    public GameObject mainCamera; // FIXME!
    private float timeInAir = 0.0f;


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        sheepLayerMask = LayerMask.GetMask("Sheep");
    }

    bool barkCondition() 
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    void Update()
    {
        // Update desired direction for sheeps in radius
        if (barkCondition()) {
            Collider[] barkSheeps = Physics.OverlapSphere(transform.position, barkDistance, sheepLayerMask);
            foreach (var sheepCollider in barkSheeps)
            {
                sheepCollider.gameObject.GetComponent<Sheep>().updateDesiredDirection(sheepCollider.gameObject.transform.position - transform.position);
            }
        }

        if (!controller.isGrounded) 
        {
            timeInAir += Time.deltaTime;
        } else 
        {
            timeInAir = 0.0f;
        }


        // Move
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 accelerationVector = Quaternion.AngleAxis(mainCamera.transform.rotation.eulerAngles.y, Vector3.up) * new Vector3(x, 0, z);
        if (accelerationVector.magnitude > 0) 
        {
            playerVelocity = controller.velocity + acceleration * accelerationVector.normalized * Time.deltaTime;
            if (playerVelocity.magnitude * Time.deltaTime < controller.minMoveDistance)
            {
                playerVelocity *= (controller.minMoveDistance / Time.deltaTime) / playerVelocity.magnitude;
            }
        } else if (controller.velocity.magnitude > 0) 
        {
            playerVelocity = controller.velocity - deceleration * controller.velocity.normalized * Time.deltaTime;
        }

        if (playerVelocity.magnitude > maxSpeed)
        {
            playerVelocity *= maxSpeed / playerVelocity.magnitude;
        }
        
        // Gravity
        playerVelocity += Vector3.down * 0.5f;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        // Rotate
        if (controller.velocity.magnitude > 0) 
        {
            float targetAngle = Mathf.Atan2(controller.velocity.x, controller.velocity.z) * Mathf.Rad2Deg + 90;// + mainCamera.transform.rotation.eulerAngles.y + 90;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }
}
