using TMPro;
using UnityEngine;

public class HitCounter : MonoBehaviour
{
    public TextMeshProUGUI hitText;

    private int hitCount = 0;
    //何故かHPが減っている
    public void AddHit()
    {
        hitCount++;
        UpdateText();
    }

    private void UpdateText()
    {
        hitText.text = "Hits: " + hitCount;
    }
    //  外部からヒット数を取得できるようにする
    public int GetHitCount()
    {
        return hitCount;
    }
}
