using UnityEngine;

public class MathPlus
{
    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f, float sigma = 1.0f)
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

    public static float IntPow(float x, int pow)
    {
        float res = 1.0f;
        for (int i = 0; i < pow; i++, res *= x);
        return res;
    }
}