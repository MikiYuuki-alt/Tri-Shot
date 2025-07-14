using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public TMP_InputField nameInput;

    public void OnStartButtonPressed()
    {
        string name = nameInput.text;

        if (!string.IsNullOrEmpty(name))
        {
            GameManager.Instance.playerName = name;
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning("–¼‘O‚ª“ü—Í‚³‚ê‚Ä‚¢‚Ü‚¹‚ñI");
        }
    }
    public void OnRankingButtonPressed()
    {
        SceneManager.LoadScene("EndScene");
    }
}
