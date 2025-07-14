using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Playershot : MonoBehaviour
{
    private Vector3 m_velocity; // ���x
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        // �ړ�����
        transform.localPosition += m_velocity;
    }
    public void Init(float angle, float speed)
    {
        // �e�̔��ˊp�x���x�N�g���ɕϊ�����
        var direction = Utils.GetDirection(angle);

        // ���ˊp�x�Ƒ������瑬�x�����߂�
        m_velocity = direction * speed;

        // �e���i�s�����������悤�ɂ���
        var angles = transform.localEulerAngles;
        angles.z = angle - 90;
        transform.localEulerAngles = angles;

        // 2 �b��ɍ폜����
        Destroy(gameObject, 2);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            // �_���[�W��^����
            other.GetComponent<Boss>()?.TakeDamage(10); // ��F10�_���[�W
            Destroy(gameObject); // �e������
        }
    }

}
