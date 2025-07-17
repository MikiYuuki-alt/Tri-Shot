using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_speed; // �ړ��̑���
    public Playershot m_shotPrefab; // �e�̃v���n�u
    public float m_shotSpeed; // �e�̈ړ��̑���
    public float m_shotAngleRange; // �����̒e�𔭎˂��鎞�̊p�x
    public float m_shotTimer; // �e�̔��˃^�C�~���O���Ǘ�����^�C�}�[
    public int m_shotCount; // �e�̔��ː�
    public float m_shotInterval; // �e�̔��ˊԊu�i�b�j
    public GameObject m_clonePrefab;  // ���g�v���n�u
    private GameObject m_leftClone; //�N���[��
    private GameObject m_rightClone;
    private bool isCloning = false;
    public AudioClip m_shotSE;              // ����SE
    private AudioSource m_audioSource;      // ���Đ��p
   



    // Start is called before the first frame update

    void Start()
        {

            // �J�����̉�ʃT�C�Y�Ɋ�Â��Ĉړ�������ݒ肷��
            var cam = Camera.main;
            float height = cam.orthographicSize;
            float width = height * cam.aspect;

            float marginX = 0.3f; // �v���C���[����ʒ[����͂ݏo���Ȃ��]��
            float marginY = 0.3f;

            Utils.m_moveLimit = new Vector2(width - marginX, height - marginY);
        
        // AudioSource ���擾�i�܂��͒ǉ��j
        m_audioSource = GetComponent<AudioSource>();
        if (m_audioSource == null)
            m_audioSource = gameObject.AddComponent<AudioSource>();
    }




    // Update is called once per frame
    void Update()
    {
        // �t���[�����[�g�Œ�
        Application.targetFrameRate = 60;

        // --- �v���C���[�ړ��i���X�e�B�b�N or �L�[�{�[�h�j ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 velocity = new Vector3(h, v) * m_speed;
        transform.localPosition += velocity;
        transform.localPosition = Utils.ClampPosition(transform.localPosition);

        // --- �����̍X�V�����i�E�X�e�B�b�N or �}�E�X�j ---
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float rx = Input.GetAxis("RightStickHorizontal");
        float ry = Input.GetAxis("RightStickVertical");

        Vector3 direction;
        if (Mathf.Abs(rx) > 0.2f || Mathf.Abs(ry) > 0.2f)
        {
            // �E�X�e�B�b�N���͂�����Ƃ��͂�������D��
            direction = new Vector3(rx, ry, 0f);
        }
        else
        {
            // �Ȃ���΃}�E�X����
            direction = Input.mousePosition - screenPos;
        }

        // �x�N�g���̒�����0����Ȃ��Ƃ��̂݉�]
        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector3 angles = transform.localEulerAngles;
            angles.z = angle - 90;
            transform.localEulerAngles = angles;
        }

        // --- ���g���o���i�E�N���b�N or RB�j ---
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.JoystickButton5)) && !isCloning)
        {
            isCloning = true;
            Vector3 offset = Vector3.right * 0.8f;
            m_leftClone = Instantiate(m_clonePrefab, transform.position - offset, transform.rotation);
            m_rightClone = Instantiate(m_clonePrefab, transform.position + offset, transform.rotation);
        }

        // --- ���g�̈ʒu���X�V ---
        if (isCloning)
        {
            Vector3 offset = Vector3.right * 0.8f;

            if (m_leftClone != null)
            {
                m_leftClone.transform.position = transform.position - offset;
                m_leftClone.transform.rotation = transform.rotation;
            }

            if (m_rightClone != null)
            {
                m_rightClone.transform.position = transform.position + offset;
                m_rightClone.transform.rotation = transform.rotation;
            }
        }

        // --- ���g�������i�E�N���b�N���� or RB�����j ---
        if ((Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.JoystickButton5)) && isCloning)
        {
            isCloning = false;
            if (m_leftClone != null) Destroy(m_leftClone);
            if (m_rightClone != null) Destroy(m_rightClone);
        }

        // --- �e�̔��ˏ��� ---
        m_shotTimer += Time.deltaTime;
        if (m_shotTimer >= m_shotInterval)
        {
            m_shotTimer = 0;

            // ���݂̌����ɒe������
            float currentAngle = transform.localEulerAngles.z + 90f;
            ShootNWay(currentAngle, m_shotAngleRange, m_shotSpeed, m_shotCount);

            // ���g������
            if (isCloning)
            {
                if (m_leftClone != null)
                    ShootClone(m_leftClone.transform.position, currentAngle);
                if (m_rightClone != null)
                    ShootClone(m_rightClone.transform.position, currentAngle);
            }
        }
    }

    // �e�𔭎˂���֐�
    private void ShootNWay(float angleBase, float angleRange, float speed, int count)
    {
        if (m_shotSE != null && m_audioSource != null)
        {
            m_audioSource.volume = 0.2f; // ���ʂ�20%�ɐݒ�
            m_audioSource.PlayOneShot(m_shotSE);
        }


        var pos = transform.localPosition; // �v���C���[�̈ʒu
        var rot = transform.localRotation; // �v���C���[�̌���

        // �e�𕡐����˂���ꍇ
        if (1 < count)
        {
            // ���˂���񐔕����[�v����
            for (int i = 0; i < count; ++i)
            {
                // �e�̔��ˊp�x���v�Z����
                var angle = angleBase +
                    angleRange * ((float)i / (count - 1) - 0.5f);

                // ���˂���e�𐶐�����
                var shot = Instantiate(m_shotPrefab, pos, rot);

                // �e�𔭎˂�������Ƒ�����ݒ肷��
                shot.Init(angle, speed);
            }
        }
        // �e�� 1 �������˂���ꍇ
        else if (count == 1)
        {
            // ���˂���e�𐶐�����
            var shot = Instantiate(m_shotPrefab, pos, rot);

            // �e�𔭎˂�������Ƒ�����ݒ肷��
            shot.Init(angleBase, speed);
        }
    }
    private void ShootClone(Vector3 pos, float angle)
    {
        // �N���[������������悤�ɒe������
        if (1 < m_shotCount)
        {
            for (int i = 0; i < m_shotCount; ++i)
            {
                var ang = angle + m_shotAngleRange * ((float)i / (m_shotCount - 1) - 0.5f);
                var shot = Instantiate(m_shotPrefab, pos, Quaternion.identity);
                shot.Init(ang, m_shotSpeed);
            }
        }
        else if (m_shotCount == 1)
        {
            var shot = Instantiate(m_shotPrefab, pos, Quaternion.identity);
            shot.Init(angle, m_shotSpeed);
        }
    }



}
