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
    [SerializeField] float delay;
    [SerializeField] float duration;
    [SerializeField] float targetValue;
    [SerializeField] Ease ease;
    [SerializeField] GameObject nrBanner;
    [Header("ResultBanners")]
    [SerializeField] GameObject @default;
    [SerializeField] GameObject newRecord;

    private void Start()
    {
        if (parent == null) parent = transform;

        nrBanner.SetActive(false);
        newRecord.SetActive(false);
        @default.SetActive(true);

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
                if (sm.GetScore() >= data.ranking.Max() && data.ranking.Count(x => x == sm.GetScore()) == 1)
                {
                    DisplayNewRecordEffect();
                }
            }
        }
        else
        {
            var t = Instantiate(textPref, parent).GetComponent<Text>();
            t.text = "NOTHING YET.";
        }
    }

    [ContextMenu(nameof(DisplayNewRecordEffect))]
    private void DisplayNewRecordEffect()
    {
        @default.SetActive(false);
        newRecord.SetActive(true);

        Invoke(nameof(AnimateNRBanner), delay);
    }

    void AnimateNRBanner()
    {
        nrBanner.SetActive(true);
        nrBanner.transform.DOScale(targetValue, duration).SetLoops(2, LoopType.Yoyo).SetEase(ease);
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
