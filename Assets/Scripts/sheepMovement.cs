using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sheepMovement : MonoBehaviour
{
    private LayerMask sheepLayerMask;
    private float walkSpeed;
    private float speed;
    public float minSpeed;
    public float maxSpeed;
    public float sigmaSpeed;
    public float viewDistance;
    public float herdFollowIntention;
    public float herdDistance;
    public float moveFadeSpeed;
    private CharacterController characterController;
    public float decreaseTime = 0.0f;

    // Testing
    public Vector3 desiredDirection = Vector3.zero;
    
    private static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f, float sigma = 1.0f)
    {
        float u, v, S, std, res;
        float mean = (minValue + maxValue) / 2.0f;
    
        do
        {
            do
            {
                u = 2.0f * UnityEngine.Random.value - 1.0f;
                v = 2.0f * UnityEngine.Random.value - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0f);
        
            // Standard Normal Distribution
            std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
        
            // Normal Distribution centered between the min and max value
            res = std * sigma + mean;
        } while (res > maxValue || res < minValue);
        
        return res;
    }

    void Start()
    {
        sheepLayerMask = LayerMask.GetMask("Sheep");
        walkSpeed = RandomGaussian(minSpeed, maxSpeed, sigmaSpeed);
        characterController = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        Collider[] seeSheeps = Physics.OverlapSphere(transform.position, viewDistance, sheepLayerMask);

        Vector3 herdFollowDirection = Vector3.zero;

        foreach (var item in seeSheeps)
        {
            GameObject sheep = item.gameObject;
            Vector3 difference = sheep.transform.position - transform.position;
            herdFollowDirection += difference.normalized * (1 - Mathf.Exp(-(difference.magnitude - herdDistance)));
        }

        Vector3 resultingDirection = desiredDirection + herdFollowDirection.normalized * herdFollowIntention;

        // Walk
        resultingDirection = (resultingDirection.normalized - new Vector3(0, resultingDirection.normalized.y, 0)).normalized;
        characterController.Move(resultingDirection * walkSpeed * Time.deltaTime);
        transform.position = transform.position - new Vector3(0, transform.position.y - 0.5f, 0);
        if (desiredDirection.magnitude != 0) {
            desiredDirection *= Mathf.Clamp(1 - moveFadeSpeed * decreaseTime, 0, 1) / desiredDirection.magnitude;
            decreaseTime += Time.deltaTime;
        }
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}

