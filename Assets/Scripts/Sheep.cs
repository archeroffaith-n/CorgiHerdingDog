using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    // Variable of state

    // Tunable parameters
    private float walkSpeed;
    private float speed;
    public float minSpeed;
    public float maxSpeed;
    public float sigmaSpeed;
    public float herdFollowIntention;
    public float randomWalkIntention;
    public float moveFadeSpeed;

    // Inner data
    private LayerMask sheepLayerMask;
    private CharacterController characterController;
    private Vector3 desiredDirection;
    private float desiredDirectionTime = 0.0f;
    private Vector3 randomWalkDirection;
    private float randomWalkTime = 0.0f;
    private Vector3 resultingDirection;
    
    private float minDistanceMaxForce = 1.0f;
    private float minDistance = 4.0f;
    private float maxDistance = 8.0f;
    private float maxDistanceMaxForce = 12.0f;
    private float maxViewDistance = 16.0f;
    private float maxMinForce = 1.0f;
    private float maxMaxForce = 1.0f;
    private float divisor1;
    private float divisor2;
    private float divisor3;

    private float HerdDistanceToForce(float distance)
    {
        if (distance <= minDistanceMaxForce)
            return maxMinForce;
        else if (distance > minDistanceMaxForce && distance <= minDistance)
            return maxMinForce / divisor1 * (-2 * MathPlus.IntPow(distance, 3) 
                   + 3 * (minDistanceMaxForce + minDistance) * MathPlus.IntPow(distance, 2)
                   - 6 * minDistanceMaxForce * minDistance * distance
                   + (3 * minDistanceMaxForce - minDistance) * MathPlus.IntPow(minDistance, 2));
        else if (distance > maxDistance && distance <= maxDistanceMaxForce)
            return maxMaxForce / divisor2 * (-2 * MathPlus.IntPow(distance, 3) 
                   + 3 * (maxDistance + maxDistanceMaxForce) * MathPlus.IntPow(distance, 2)
                   - 6 * maxDistance * maxDistanceMaxForce * distance
                   - (maxDistance - 3 * maxDistanceMaxForce) * MathPlus.IntPow(maxDistance, 2));
        else if (distance > maxDistanceMaxForce && distance <= maxViewDistance)
            return maxMaxForce / divisor3 * (2 * MathPlus.IntPow(distance, 3) 
                   - 3 * (maxDistanceMaxForce + maxViewDistance) * MathPlus.IntPow(distance, 2)
                   + 6 * maxDistanceMaxForce * maxViewDistance * distance
                   - (3 * maxDistanceMaxForce - maxViewDistance) * MathPlus.IntPow(maxViewDistance, 2));
        else
            return 0.0f;
    }

    void Start()
    {
        sheepLayerMask = LayerMask.GetMask("Sheep");
        walkSpeed = MathPlus.RandomGaussian(minSpeed, maxSpeed, sigmaSpeed);
        characterController = gameObject.GetComponent<CharacterController>();
        divisor1 = MathPlus.IntPow(minDistanceMaxForce - minDistance, 3);
        divisor2 = MathPlus.IntPow(maxDistance - maxDistanceMaxForce, 3);
        divisor3 = MathPlus.IntPow(maxDistanceMaxForce - maxViewDistance, 3);
    }

    public void updateDesiredDirection(Vector3 direction)
    {
        desiredDirection = direction.normalized;
        desiredDirectionTime = 0.0f;
    }

    void Update()
    {
        Collider[] seeSheeps = Physics.OverlapSphere(transform.position, maxViewDistance, sheepLayerMask);

        Vector3 herdFollowDirection = Vector3.zero;

        foreach (var item in seeSheeps)
        {
            GameObject sheep = item.gameObject;
            Vector3 difference = sheep.transform.position - transform.position;
            herdFollowDirection += -difference.normalized * HerdDistanceToForce(difference.magnitude);
        }

        if (randomWalkTime <= 0.0f){
            randomWalkTime = Random.Range(1.0f, 2.0f);
            randomWalkDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized;
        }
        randomWalkTime -= Time.deltaTime;

        resultingDirection = desiredDirection + herdFollowDirection.normalized * herdFollowIntention + randomWalkDirection * randomWalkIntention;

        // Walk
        resultingDirection = resultingDirection.normalized + Vector3.down * 0.5f;
        characterController.Move(resultingDirection * walkSpeed * Time.deltaTime);

        if (desiredDirection.magnitude != 0) {
            desiredDirection *= Mathf.Clamp(1 - moveFadeSpeed * desiredDirectionTime, 0, 1) / desiredDirection.magnitude;
            desiredDirectionTime += Time.deltaTime;
        }
    }

    void OnDrawGizmos() 
    {
        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(transform.position, maxViewDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + desiredDirection * 4.0f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + resultingDirection * 4.0f);
    }
}

