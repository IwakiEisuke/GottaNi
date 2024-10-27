using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ランキングの表示処理を行うクラス
/// </summary>
public class RankingBoard : MonoBehaviour
{
    [SerializeField] Transform parent;
    [SerializeField] RankingData data;
    [SerializeField] Text currentScoreText;
    [SerializeField] GameObject textPref;
    [SerializeField] int maxCount;

    [Header("NewRecordEffectSettings")]
    [SerializeField] Animator animator;

    [Header("Debug")]
    [SerializeField] bool playNewRecordEffect;

    private void Start()
    {
        if (parent == null) parent = transform;

        if (animator) animator.Play("NewRecordInit");

        if (playNewRecordEffect)
        {
            PlayNewRecordEffect();
        }

        DestroyChildren();
        DisplayRanking();
    }

    private void DisplayRanking()
    {
        data = Ranking.GetRanking();

        if (!(data == null || data.ranking.Count == 0))
        {
            StartCoroutine(UpdateRankingText());

            var sm = FindAnyObjectByType<ScoreManager>();
            if (sm && currentScoreText)
            {
                UpdateCurrentText(sm.GetScore());

                // ローカルレコード更新表示
                if (animator)
                {
                    var highest = sm.GetScore() >= data.ranking.Max();
                    var unique = data.ranking.Count(x => x == sm.GetScore()) == 1; // 同率一位じゃない

                    if (highest && unique)
                    {
                        PlayNewRecordEffect();
                    }
                }
            }
        }
        else
        {
            var t = Instantiate(textPref, parent).GetComponent<Text>();
            t.text = "NOTHING YET.";
        }
    }

    [ContextMenu(nameof(PlayNewRecordEffect))]
    private void PlayNewRecordEffect()
    {
        animator.Play("NewRecord");
    }

    private IEnumerator UpdateRankingText()
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

    private void UpdateCurrentText(int score)
    {
        currentScoreText.text = $"You : {score:0000}";
    }

    private void DestroyChildren()
    {
        foreach (Transform t in parent)
        {
            Destroy(t.gameObject);
        }
    }
}
