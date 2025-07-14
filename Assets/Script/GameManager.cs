using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string playerName;
    public int hitCount;

    // �����L���O�p
    public List<ScoreEntry> ranking = new List<ScoreEntry>();


    //���O�ƃX�R�A�ێ�
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

        // ���Ȃ����ɕ��ёւ��i�������q�b�g������ʁj
        ranking.Sort((a, b) => a.hits.CompareTo(b.hits));

        // ���10���ɐ���
        if (ranking.Count > 10)
        {
            ranking = ranking.GetRange(0, 10);
        }
    }

}
