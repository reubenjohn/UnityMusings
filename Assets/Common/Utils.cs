using UnityEngine;

public static class Utils
{
    public static float Lerp(float a, float b, float t) => a + (b - a) * Mathf.Clamp(t, 0, 1);
}