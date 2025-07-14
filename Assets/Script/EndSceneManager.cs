using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    public TextMeshProUGUI rankingText;

    // ƒ{ƒ^ƒ“‚ÉŠ„‚è“–‚Ä‚éŠÖ”
   

    public void OnBackToTitlePressed()
    {
        SceneManager.LoadScene("TitleScene");
    }

    void Start()
    {
        rankingText.text = "";

        var ranking = GameManager.Instance.ranking;

        for (int i = 0; i < ranking.Count; i++)
        {
            var entry = ranking[i];
            rankingText.text += $"{i + 1}. {entry.name} - {entry.hits} hits\n";
        }
    }
}
