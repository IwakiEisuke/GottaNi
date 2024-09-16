using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RankingBoard : MonoBehaviour
{
    [SerializeField] Transform parent;
    [SerializeField] RankingData data;
    [SerializeField] Text currentScoreText;
    [SerializeField] GameObject textPref;
    [SerializeField] int maxCount;

    void Start()
    {
        if (parent == null) parent = transform;

        DestroyChildren();
        Generate();
    }

    void Generate()
    {
        data = Ranking.GetRanking();

        if (!(data == null || data.ranking.Count == 0))
        {
            StartCoroutine(GenerateRanking());

            var sm = FindAnyObjectByType<ScoreManager>();
            if (sm && currentScoreText)
            {
                GenerateCurrent(sm.GetScore());
            }
        }
        else
        {
            var t = Instantiate(textPref, parent).GetComponent<Text>();
            t.text = "NOTHING YET.";
        }
    }

    private IEnumerator GenerateRanking()
    {
        var ranking = data.ranking.OrderByDescending(x => x).ToArray();
        var count = Mathf.Min(maxCount, ranking.Length);

        for (int i = 0; i < count; i++)
        {
            var t = Instantiate(textPref, parent).GetComponent<Text>();
            t.text = $"{i + 1} : {ranking[i]:0000}";
            yield return null;
        }
    }

    public void GenerateCurrent(int score)
    {
        currentScoreText.text = $"You : {score:0000}";
    }

    void DestroyChildren()
    {
        foreach (Transform t in parent)
        {
            Destroy(t.gameObject);
        }
    }
}
