using UnityEngine;

public static class Helper
{
    public static float GetDepht(Vector3 position)
    {
        return Mathf.Abs(Camera.main.transform.position.z - position.z);
    }

    public static float Normalization(float position, float min, float max)
    {
        return (position - min) / (max - min);
    }

    public static float InverseNormalization(float position, float min, float max)
    {
        return min + (max - min) * position;
    }

    public static float ViewportToWord(float position, float depth)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(position, 0, depth)).x;
    }

    public static float ViewportToWord(float position, float min, float max, float depth)
    {
        position = InverseNormalization(position, min, max);
        return Camera.main.ViewportToWorldPoint(new Vector3(position, 0, depth)).x;
    }

    public static float WorldToViewport(Vector3 position)
    {
        Vector3 viewport = Camera.main.WorldToViewportPoint(position);
        return viewport.x;
    }

    public static float WorldToViewport(Vector3 position, float min, float max)
    {
        return Normalization(WorldToViewport(position), min, max);
    }

    public static float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    public static float Clamp(float value, float min, float max)
    {
        if (value < min)
            return min;

        if (value > max)
            return max;

        return value;
    }
}
