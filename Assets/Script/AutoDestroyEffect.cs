using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    private Animator animator;
    private float animationLength;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // 現在のアニメーションの長さを取得
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                animationLength = clips[0].length;
                // 一定時間後にDestroyを呼ぶ
                Destroy(gameObject, animationLength);
                Debug.Log("エフェクト破壊");
            }
            else
            {
                // クリップがないなら適当な時間で消す
                Destroy(gameObject, 1f);
            }
        }
        else
        {
            // Animatorがないなら1秒後に消す
            Destroy(gameObject, 1f);
        }
    }
}
