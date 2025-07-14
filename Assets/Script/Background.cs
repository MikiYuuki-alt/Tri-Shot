using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    public float scrollSpeed = 2f;  // �X�N���[�����x
    public float imageHeight = 10f; // �w�i�摜�̍���

    private void Update()
    {
        // �������Ɉړ�
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        // ���̈ʒu�܂ŉ����������ɖ߂�
        if (transform.position.y <= -imageHeight)
        {
            transform.position += new Vector3(0, imageHeight * 2f, 0);
        }
    }
}
