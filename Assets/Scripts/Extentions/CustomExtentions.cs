using UnityEngine;

public static class CustomExtentions
{
    public static Vector2 Rotate(this Vector2 origin, float degree)
    {
        float angleRadians = degree * Mathf.Deg2Rad; // 각도를 라디안으로 변환
        float cos = Mathf.Cos(angleRadians);
        float sin = Mathf.Sin(angleRadians);

        float newX = origin.x * cos - origin.y * sin;
        float newY = origin.x * sin + origin.y * cos;

        return new Vector2(newX, newY);
    }
}