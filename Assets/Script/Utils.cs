using UnityEngine;
//よくわからん
public static class Utils
{
    public static Vector2 m_moveLimit;

    // 位置制限
    public static Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, -m_moveLimit.x, m_moveLimit.x);
        position.y = Mathf.Clamp(position.y, -m_moveLimit.y, m_moveLimit.y);
        return position;
    }

    // 2点間の角度（度）
    public static float GetAngle(Vector3 from, Vector3 to)
    {
        Vector3 diff = to - from;
        return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
    }

    // 角度から方向ベクトルを計算
    public static Vector3 GetDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
