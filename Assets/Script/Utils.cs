using UnityEngine;
//�悭�킩���
public static class Utils
{
    public static Vector2 m_moveLimit;

    // �ʒu����
    public static Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, -m_moveLimit.x, m_moveLimit.x);
        position.y = Mathf.Clamp(position.y, -m_moveLimit.y, m_moveLimit.y);
        return position;
    }

    // 2�_�Ԃ̊p�x�i�x�j
    public static float GetAngle(Vector3 from, Vector3 to)
    {
        Vector3 diff = to - from;
        return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
    }

    // �p�x��������x�N�g�����v�Z
    public static Vector3 GetDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
