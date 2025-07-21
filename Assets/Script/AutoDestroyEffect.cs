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
            // ���݂̃A�j���[�V�����̒������擾
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                animationLength = clips[0].length;
                // ��莞�Ԍ��Destroy���Ă�
                Destroy(gameObject, animationLength);
                Debug.Log("�G�t�F�N�g�j��");
            }
            else
            {
                // �N���b�v���Ȃ��Ȃ�K���Ȏ��Ԃŏ���
                Destroy(gameObject, 1f);
            }
        }
        else
        {
            // Animator���Ȃ��Ȃ�1�b��ɏ���
            Destroy(gameObject, 1f);
        }
    }
}
