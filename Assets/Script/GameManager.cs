using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string playerName;
    public int hitCount;

    // ランキング用
    public List<ScoreEntry> ranking = new List<ScoreEntry>();


    //名前とスコア保持
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class ScoreEntry
    {
        public string name;
        public int hits;

        public ScoreEntry(string name, int hits)
        {
            this.name = name;
            this.hits = hits;
        }
    }

    public void AddScore()
    {
        ranking.Add(new ScoreEntry(playerName, hitCount));

        // 少ない順に並び替え（小さいヒット数が上位）
        ranking.Sort((a, b) => a.hits.CompareTo(b.hits));

        // 上位10件に制限
        if (ranking.Count > 10)
        {
            ranking = ranking.GetRange(0, 10);
        }
    }

}
