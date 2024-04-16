using UnityEngine;

public static class MathExtensions
{
    public static Vector3 Abs(this Vector3 vector)
    {
        vector.Set(vector.x.Abs(), vector.y.Abs(), vector.z.Abs());
        return vector;
    }

    public static bool IsEven(this int value) => value % 2 == 0;

    public static float Abs(this float value) => Mathf.Abs(value);
}
