using UnityEngine;


public class UIButtonSE : MonoBehaviour
{
    public AudioClip clickSE;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayClickSound()
    {
        if (clickSE != null)
        {
            audioSource.PlayOneShot(clickSE);
        }
    }
}
