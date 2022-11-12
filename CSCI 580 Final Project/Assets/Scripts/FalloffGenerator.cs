using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }

    public static float[,] GenerateCircularFalloffMap(int size, float circleRadius, float gradientRadius)
    {
        float[,] map = new float[size, size];
        Vector2 center = new Vector2(size/2,size/2);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector2 point = new Vector2(i, j);
                if(Vector2.Distance(center,point) <= circleRadius)
                {
                    if(Vector2.Distance(center,point) <= gradientRadius)
                    {
                        map[i, j] = 0;
                    }
                    else
                    {
                        float totallength = circleRadius - gradientRadius;
                        float curlength = Vector2.Distance(center, point) - gradientRadius;
                        float fraction = curlength / totallength;
                        map[i, j] = Mathf.Lerp(0, 1, fraction);
                    }
                }
                else
                {
                    map[i, j] = 1;
                }
            }
        }
        return map;
    }

    static float Evaluate(float value)
    {
        float a = 4;
        float b = 4;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}