using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_speed; // 移動の速さ
    public Playershot m_shotPrefab; // 弾のプレハブ
    public float m_shotSpeed; // 弾の移動の速さ
    public float m_shotAngleRange; // 複数の弾を発射する時の角度
    public float m_shotTimer; // 弾の発射タイミングを管理するタイマー
    public int m_shotCount; // 弾の発射数
    public float m_shotInterval; // 弾の発射間隔（秒）
    public GameObject m_clonePrefab;  // 分身プレハブ
    private GameObject m_leftClone; //クローン
    private GameObject m_rightClone;
    private bool isCloning = false;
    public AudioClip m_shotSE;              // 発射SE
    private AudioSource m_audioSource;      // 音再生用
   



    // Start is called before the first frame update

    void Start()
        {

            // カメラの画面サイズに基づいて移動制限を設定する
            var cam = Camera.main;
            float height = cam.orthographicSize;
            float width = height * cam.aspect;

            float marginX = 0.3f; // プレイヤーが画面端からはみ出さない余白
            float marginY = 0.3f;

            Utils.m_moveLimit = new Vector2(width - marginX, height - marginY);
        
        // AudioSource を取得（または追加）
        m_audioSource = GetComponent<AudioSource>();
        if (m_audioSource == null)
            m_audioSource = gameObject.AddComponent<AudioSource>();
    }




    // Update is called once per frame
    void Update()
    {
        // フレームレート固定
        Application.targetFrameRate = 60;

        // --- プレイヤー移動（左スティック or キーボード） ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 velocity = new Vector3(h, v) * m_speed;
        transform.localPosition += velocity;
        transform.localPosition = Utils.ClampPosition(transform.localPosition);

        // --- 向きの更新処理（右スティック or マウス） ---
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float rx = Input.GetAxis("RightStickHorizontal");
        float ry = Input.GetAxis("RightStickVertical");

        Vector3 direction;
        if (Mathf.Abs(rx) > 0.2f || Mathf.Abs(ry) > 0.2f)
        {
            // 右スティック入力があるときはそっちを優先
            direction = new Vector3(rx, ry, 0f);
        }
        else
        {
            // なければマウス方向
            direction = Input.mousePosition - screenPos;
        }

        // ベクトルの長さが0じゃないときのみ回転
        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector3 angles = transform.localEulerAngles;
            angles.z = angle - 90;
            transform.localEulerAngles = angles;
        }

        // --- 分身を出す（右クリック or RB） ---
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.JoystickButton5)) && !isCloning)
        {
            isCloning = true;
            Vector3 offset = Vector3.right * 0.8f;
            m_leftClone = Instantiate(m_clonePrefab, transform.position - offset, transform.rotation);
            m_rightClone = Instantiate(m_clonePrefab, transform.position + offset, transform.rotation);
        }

        // --- 分身の位置を更新 ---
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

        // --- 分身を消す（右クリック離す or RB離す） ---
        if ((Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.JoystickButton5)) && isCloning)
        {
            isCloning = false;
            if (m_leftClone != null) Destroy(m_leftClone);
            if (m_rightClone != null) Destroy(m_rightClone);
        }

        // --- 弾の発射処理 ---
        m_shotTimer += Time.deltaTime;
        if (m_shotTimer >= m_shotInterval)
        {
            m_shotTimer = 0;

            // 現在の向きに弾を撃つ
            float currentAngle = transform.localEulerAngles.z + 90f;
            ShootNWay(currentAngle, m_shotAngleRange, m_shotSpeed, m_shotCount);

            // 分身も撃つ
            if (isCloning)
            {
                if (m_leftClone != null)
                    ShootClone(m_leftClone.transform.position, currentAngle);
                if (m_rightClone != null)
                    ShootClone(m_rightClone.transform.position, currentAngle);
            }
        }
    }

    // 弾を発射する関数
    private void ShootNWay(float angleBase, float angleRange, float speed, int count)
    {
        if (m_shotSE != null && m_audioSource != null)
        {
            m_audioSource.volume = 0.2f; // 音量を20%に設定
            m_audioSource.PlayOneShot(m_shotSE);
        }


        var pos = transform.localPosition; // プレイヤーの位置
        var rot = transform.localRotation; // プレイヤーの向き

        // 弾を複数発射する場合
        if (1 < count)
        {
            // 発射する回数分ループする
            for (int i = 0; i < count; ++i)
            {
                // 弾の発射角度を計算する
                var angle = angleBase +
                    angleRange * ((float)i / (count - 1) - 0.5f);

                // 発射する弾を生成する
                var shot = Instantiate(m_shotPrefab, pos, rot);

                // 弾を発射する方向と速さを設定する
                shot.Init(angle, speed);
            }
        }
        // 弾を 1 つだけ発射する場合
        else if (count == 1)
        {
            // 発射する弾を生成する
            var shot = Instantiate(m_shotPrefab, pos, rot);

            // 弾を発射する方向と速さを設定する
            shot.Init(angleBase, speed);
        }
    }
    private void ShootClone(Vector3 pos, float angle)
    {
        // クローンからも同じように弾を撃つ
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
