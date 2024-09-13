using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RankingBoard : MonoBehaviour
{
    [SerializeField] RankingData data;
    [SerializeField] Text currentScoreText;
    [SerializeField] GameObject textPref;
    [SerializeField] int maxcount;

    void Start()
    {
        DestroyChildren();
        Generate();
    }

    void Generate()
    {
        data = Ranking.GetRanking();

        if (!(data == null || data.ranking.Count == 0))
        {
            var ranking = data.ranking.OrderByDescending(x => x).ToArray();
            var count = Mathf.Min(maxcount, ranking.Length);

            for (int i = 0; i < count; i++)
            {
                var t = Instantiate(textPref, transform).GetComponent<Text>();
                t.text = $"{i + 1} : {ranking[i]:0000}";
            }

            var sm = FindAnyObjectByType<ScoreManager>();
            if (sm && currentScoreText)
            {
                GenerateCurrent(sm.GetScore());
            }
        }
        else
        {
            var t = Instantiate(textPref, transform).GetComponent<Text>();
            t.text = "NOTHING YET.";
        }
    }

    public void GenerateCurrent(int score)
    {
        currentScoreText.text = $"You : {score:0000}";
    }

    void DestroyChildren()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }
}
