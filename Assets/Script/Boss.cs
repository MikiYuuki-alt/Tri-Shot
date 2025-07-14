using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("HP設定")]
    public int maxHP = 100;
    private int currentHP;
    private bool isDead = false;

    [Header("HPバーUI")]
    public Slider hpBar;
    private Image hpFillImage;

    [Header("演出")]
    public Color normalColor = Color.red;
    public Color flashColor = Color.white;
    public float flashInterval = 0.3f;
    private Coroutine flashCoroutine;

    [Header("弾幕設定（通常）")]
    public GameObject shotPrefab;
    public float shotSpeed = 5f;
    public int shotCount = 8;
    public float shotAngleRange = 90f;
    public float shotInterval = 1.5f;

    [Header("弾幕設定（怒りモード）")]
    public GameObject fastShotPrefab;
    public float fastShotSpeed = 8f;
    public float fastShotInterval = 0.7f;
    public float fastShotAngleRange = 120f;
    public int fastShotCount = 12; 

    [Header("怒り時追加弾幕")]
    public GameObject extraShotPrefab;
    public float extraShotSpeed = 6f;
    public float extraShotInterval = 2.0f;
    public int extraShotCount = 6;
    public float extraShotAngleRange = 60f;

    [Header("スプライト切り替え")]
    public Sprite normalSprite;
    public Sprite angrySprite;
    private SpriteRenderer spriteRenderer;

    [Header("怒り時エフェクト")]
    public GameObject angryEffectPrefab;

    [Header("怒り時エフェクトの位置調整")]
    public Vector3 effectOffset = new Vector3(0f, 1f, 0f);

    [Header("移動設定（左右往復）")]
    public float moveRange = 3f;  // 左右移動幅
    public float moveSpeed = 2f;  // 移動スピード（速さ）
    private Vector3 startPos;     // 初期位置を保存

    [Header("爆発エフェクト")]
    public GameObject explosionEffectPrefab;

    [Header("爆発SE")]
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
        
        // 追加：左右にゆっくり往復移動
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
            // 怒りモード：速い弾
            if (shotTimer >= fastShotInterval)
            {
                shotTimer = 0f;
                ShootNWay(270f, fastShotPrefab, fastShotSpeed, fastShotCount, fastShotAngleRange);
            }

            // 怒りモード：追加弾（extra）
            extraShotTimer += Time.deltaTime;
            if (extraShotTimer >= extraShotInterval)
            {
                extraShotTimer = 0f;

                // プレイヤーの方向に向けて発射
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
        extraShotTimer = 0f; // ←タイマー初期化
        Debug.Log("怒りモード突入！");

        if (spriteRenderer && angrySprite)
        {
            spriteRenderer.sprite = angrySprite;
        }
        if (angryEffectPrefab != null)
        {
            // ボスの真上にエフェクトを再生
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

        Debug.Log("ボスを倒した！");
        // ヒットストップ効果
        StartCoroutine(DeathSequence());

       
    }
    private IEnumerator DeathSequence()
    {
        // 1. 画面停止（完全に時間を止める）
        Time.timeScale = 0f;

        // 2. ボスの振動演出（リアルタイムで動く）
        float duration = 0.5f; // 振動時間
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
        // ボスのスプライトを非表示にする
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // 3. 爆発エフェクト
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            if (explosionSE != null)
            {
                // 効果音をその場で再生（AudioSource不要）
                AudioSource.PlayClipAtPoint(explosionSE, transform.position, 0.7f);
            }
        }

        // 4. 少し待つ
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;

        // 5. スコア・シーン遷移
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
