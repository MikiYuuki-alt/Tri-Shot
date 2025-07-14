using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public AudioClip titleBGM;
    public AudioClip gameBGM;
    public AudioClip endBGM;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private static BGMManager instance;

    [Header("フェード設定")]
    public float fadeDuration = 0.2f; // 秒

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "TitleScene":
                PlayBGM(titleBGM, 0.2f); // タイトル音量小さめ
                break;
            case "GameScene":
                PlayBGM(gameBGM, 1.0f);
                break;
            case "EndScene":
                PlayBGM(endBGM, 0.8f);
                break;
        }
    }

    private void PlayBGM(AudioClip newClip, float volume)
    {
        if (newClip == null) return;

        if (audioSource.clip == newClip && audioSource.isPlaying)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewBGM(newClip, volume));
    }

    private IEnumerator FadeToNewBGM(AudioClip newClip, float targetVolume)
    {
        float startVolume = audioSource.volume;

        // フェードアウト
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0f;

        audioSource.clip = newClip;
        audioSource.Play();

        // フェードイン
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;

        fadeCoroutine = null;
    }
}
