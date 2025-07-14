using TMPro;
using UnityEngine;

public class HitCounter : MonoBehaviour
{
    public TextMeshProUGUI hitText;

    private int hitCount = 0;
    //���̂�HP�������Ă���
    public void AddHit()
    {
        hitCount++;
        UpdateText();
    }

    private void UpdateText()
    {
        hitText.text = "Hits: " + hitCount;
    }
    //  �O������q�b�g�����擾�ł���悤�ɂ���
    public int GetHitCount()
    {
        return hitCount;
    }
}
