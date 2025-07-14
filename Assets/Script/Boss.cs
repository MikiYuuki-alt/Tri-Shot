using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("HP�ݒ�")]
    public int maxHP = 100;
    private int currentHP;
    private bool isDead = false;

    [Header("HP�o�[UI")]
    public Slider hpBar;
    private Image hpFillImage;

    [Header("���o")]
    public Color normalColor = Color.red;
    public Color flashColor = Color.white;
    public float flashInterval = 0.3f;
    private Coroutine flashCoroutine;

    [Header("�e���ݒ�i�ʏ�j")]
    public GameObject shotPrefab;
    public float shotSpeed = 5f;
    public int shotCount = 8;
    public float shotAngleRange = 90f;
    public float shotInterval = 1.5f;

    [Header("�e���ݒ�i�{�胂�[�h�j")]
    public GameObject fastShotPrefab;
    public float fastShotSpeed = 8f;
    public float fastShotInterval = 0.7f;
    public float fastShotAngleRange = 120f;
    public int fastShotCount = 12; 

    [Header("�{�莞�ǉ��e��")]
    public GameObject extraShotPrefab;
    public float extraShotSpeed = 6f;
    public float extraShotInterval = 2.0f;
    public int extraShotCount = 6;
    public float extraShotAngleRange = 60f;

    [Header("�X�v���C�g�؂�ւ�")]
    public Sprite normalSprite;
    public Sprite angrySprite;
    private SpriteRenderer spriteRenderer;

    [Header("�{�莞�G�t�F�N�g")]
    public GameObject angryEffectPrefab;

    [Header("�{�莞�G�t�F�N�g�̈ʒu����")]
    public Vector3 effectOffset = new Vector3(0f, 1f, 0f);

    [Header("�ړ��ݒ�i���E�����j")]
    public float moveRange = 3f;  // ���E�ړ���
    public float moveSpeed = 2f;  // �ړ��X�s�[�h�i�����j
    private Vector3 startPos;     // �����ʒu��ۑ�

    [Header("�����G�t�F�N�g")]
    public GameObject explosionEffectPrefab;

    [Header("����SE")]
    public AudioClip explosionSE;




    private float shotTimer;
    private float extraShotTimer;
    private bool isAngry = false;

    void Start()
    {
        currentHP = maxHP;

        if (hpBar != null)
        {
            hpBar.maxValue = maxHP;
            hpBar.value = currentHP;
            hpFillImage = hpBar.fillRect.GetComponent<Image>();
            hpFillImage.color = normalColor;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer && normalSprite)
        {
            spriteRenderer.sprite = normalSprite;
        }
        startPos = transform.position;
    }

    void Update()
    {
        
        // �ǉ��F���E�ɂ�����艝���ړ�
        float x = Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(startPos.x + x, transform.position.y, transform.position.z);
        shotTimer += Time.deltaTime;

        if (!isAngry)
        {
            if (shotTimer >= shotInterval)
            {
                shotTimer = 0f;
                ShootNWay(0f, shotPrefab, shotSpeed, shotCount, shotAngleRange);
            }
        }
        else
        {
            // �{�胂�[�h�F�����e
            if (shotTimer >= fastShotInterval)
            {
                shotTimer = 0f;
                ShootNWay(270f, fastShotPrefab, fastShotSpeed, fastShotCount, fastShotAngleRange);
            }

            // �{�胂�[�h�F�ǉ��e�iextra�j
            extraShotTimer += Time.deltaTime;
            if (extraShotTimer >= extraShotInterval)
            {
                extraShotTimer = 0f;

                // �v���C���[�̕����Ɍ����Ĕ���
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    Vector3 toPlayer = player.transform.position - transform.position;
                    float angleToPlayer = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

                    ShootNWay(angleToPlayer, extraShotPrefab, extraShotSpeed, extraShotCount, extraShotAngleRange);
                }
            }

        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        if (hpBar != null)
        {
            hpBar.value = currentHP;
        }

        if (!isAngry && currentHP <= maxHP * 0.5f)
        {
            EnterAngryMode();
        }

        if (currentHP <= maxHP * 0.2f && flashCoroutine == null)
        {
            flashCoroutine = StartCoroutine(FlashHPBar());
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void EnterAngryMode()
    {
        
        isAngry = true;
        extraShotTimer = 0f; // ���^�C�}�[������
        Debug.Log("�{�胂�[�h�˓��I");

        if (spriteRenderer && angrySprite)
        {
            spriteRenderer.sprite = angrySprite;
        }
        if (angryEffectPrefab != null)
        {
            // �{�X�̐^��ɃG�t�F�N�g���Đ�
            Vector3 effectPos = transform.position + effectOffset;
            Instantiate(angryEffectPrefab, effectPos, Quaternion.identity);
        }
    }

    private IEnumerator FlashHPBar()
    {
        while (true)
        {
            if (hpFillImage != null)
                hpFillImage.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            if (hpFillImage != null)
                hpFillImage.color = normalColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private void ShootNWay(float centerAngle, GameObject prefab, float speed, int count, float angleRange)
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        if (count <= 1)
        {
            GameObject shot = Instantiate(prefab, pos, rot);
            shot.GetComponent<BossShot>().Init(centerAngle, speed);
        }
        else
        {
            for (int i = 0; i < count; ++i)
            {
                float angle = centerAngle + angleRange * ((float)i / (count - 1) - 0.5f);
                GameObject shot = Instantiate(prefab, pos, rot);
                shot.GetComponent<BossShot>().Init(angle, speed);
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("�{�X��|�����I");
        // �q�b�g�X�g�b�v����
        StartCoroutine(DeathSequence());

       
    }
    private IEnumerator DeathSequence()
    {
        // 1. ��ʒ�~�i���S�Ɏ��Ԃ��~�߂�j
        Time.timeScale = 0f;

        // 2. �{�X�̐U�����o�i���A���^�C���œ����j
        float duration = 0.5f; // �U������
        float shakeAmount = 0.1f;
        float elapsed = 0f;
        Vector3 originalPos = transform.position;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-shakeAmount, shakeAmount);
            transform.position = originalPos + new Vector3(offsetX, 0f, 0f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.position = originalPos;
        // �{�X�̃X�v���C�g���\���ɂ���
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // 3. �����G�t�F�N�g
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            if (explosionSE != null)
            {
                // ���ʉ������̏�ōĐ��iAudioSource�s�v�j
                AudioSource.PlayClipAtPoint(explosionSE, transform.position, 0.7f);
            }
        }

        // 4. �����҂�
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;

        // 5. �X�R�A�E�V�[���J��
        var counter = FindObjectOfType<HitCounter>();
        if (counter != null)
        {
            GameManager.Instance.hitCount = counter.GetHitCount();
        }

        GameManager.Instance.AddScore();

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        SceneManager.LoadScene("EndScene");
    }
   
    


}
