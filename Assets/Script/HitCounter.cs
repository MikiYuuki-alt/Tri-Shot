using TMPro;
using UnityEngine;

public class HitCounter : MonoBehaviour
{
    public TextMeshProUGUI hitText;

    private int hitCount = 0;
    //‰½ŒÌ‚©HP‚ªŒ¸‚Á‚Ä‚¢‚é
    public void AddHit()
    {
        hitCount++;
        UpdateText();
    }

    private void UpdateText()
    {
        hitText.text = "Hits: " + hitCount;
    }
    //  ŠO•”‚©‚çƒqƒbƒg”‚ğæ“¾‚Å‚«‚é‚æ‚¤‚É‚·‚é
    public int GetHitCount()
    {
        return hitCount;
    }
}
