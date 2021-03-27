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
    public float herdFollowIntention;
    public float randomWalkIntention;
    public float moveFadeSpeed;
    private CharacterController characterController;
    public float decreaseTime = 0.0f;
    public GameObject prefab;

    // Testing
    public Vector3 desiredDirection = Vector3.zero;
    public Vector3 randomWalkDirection;
    public float randomWalkTime = 0.0f;
    
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
    private static float IntPow(float x, int pow)
    {
        float res = 1.0f;
        for (int i = 0; i < pow; i++, res *= x);
        return res;
    }
    
    private static float minDistanceMaxForce = 1.0f;
    private static float minDistance = 4.0f;
    private static float maxDistance = 8.0f;
    private static float maxDistanceMaxForce = 12.0f;
    private static float maxViewDistance = 16.0f;
    private static float maxMinForce = 1.0f;
    private static float maxMaxForce = 1.0f;
    private static float divisor1 = IntPow(minDistanceMaxForce - minDistance, 3);
    private static float divisor2 = IntPow(maxDistance - maxDistanceMaxForce, 3);
    private static float divisor3 = IntPow(maxDistanceMaxForce - maxViewDistance, 3);
    private static float HerdDistanceToForce(float distance)
    {
        if (distance <= minDistanceMaxForce)
            return maxMinForce;
        else if (distance > minDistanceMaxForce && distance <= minDistance)
            return maxMinForce / divisor1 * (-2 * IntPow(distance, 3) 
                   + 3 * (minDistanceMaxForce + minDistance) * IntPow(distance, 2)
                   - 6 * minDistanceMaxForce * minDistance * distance
                   + (3 * minDistanceMaxForce - minDistance) * IntPow(minDistance, 2));
        else if (distance > maxDistance && distance <= maxDistanceMaxForce)
            return maxMaxForce / divisor2 * (-2 * IntPow(distance, 3) 
                   + 3 * (maxDistance + maxDistanceMaxForce) * IntPow(distance, 2)
                   - 6 * maxDistance * maxDistanceMaxForce * distance
                   - (maxDistance - 3 * maxDistanceMaxForce) * IntPow(maxDistance, 2));
        else if (distance > maxDistanceMaxForce && distance <= maxViewDistance)
            return maxMaxForce / divisor3 * (2 * IntPow(distance, 3) 
                   - 3 * (maxDistanceMaxForce + maxViewDistance) * IntPow(distance, 2)
                   + 6 * maxDistanceMaxForce * maxViewDistance * distance
                   - (3 * maxDistanceMaxForce - maxViewDistance) * IntPow(maxViewDistance, 2));
        else
            return 0.0f;
    }

    void Start()
    {
        sheepLayerMask = LayerMask.GetMask("Sheep");
        walkSpeed = RandomGaussian(minSpeed, maxSpeed, sigmaSpeed);
        characterController = gameObject.GetComponent<CharacterController>();
        // for (int i = 0; i < 100; i++) {
        //     float x = i / 100.0f * 2 * maxViewDistance;
        //     Debug.Log(x);
        //     Debug.Log(HerdDistanceToForce(x));
        //     Instantiate(prefab, new Vector3(x, HerdDistanceToForce(x), 0.0f), Quaternion.identity);
        // }
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

        Vector3 resultingDirection = desiredDirection + herdFollowDirection.normalized * herdFollowIntention + randomWalkDirection * randomWalkIntention;

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
        Gizmos.DrawWireSphere(transform.position, maxViewDistance);
    }
}

