using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Playershot : MonoBehaviour
{
    private Vector3 m_velocity; // 速度
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        // 移動する
        transform.localPosition += m_velocity;
    }
    public void Init(float angle, float speed)
    {
        // 弾の発射角度をベクトルに変換する
        var direction = Utils.GetDirection(angle);

        // 発射角度と速さから速度を求める
        m_velocity = direction * speed;

        // 弾が進行方向を向くようにする
        var angles = transform.localEulerAngles;
        angles.z = angle - 90;
        transform.localEulerAngles = angles;

        // 2 秒後に削除する
        Destroy(gameObject, 2);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            // ダメージを与える
            other.GetComponent<Boss>()?.TakeDamage(10); // 例：10ダメージ
            Destroy(gameObject); // 弾を消す
        }
    }

}
