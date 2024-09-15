using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingDebugger : MonoBehaviour
{
    [ContextMenuItem(nameof(Delete), nameof(Delete))]
    [ContextMenuItem(nameof(ResetRanking), nameof(ResetRanking))]
    [ContextMenuItem(nameof(Set), nameof(Set))]
    [SerializeField] string aaa;
    [SerializeField] RankingData data;

    [ContextMenu("Delete")]
    public void Delete()
    {
        PlayerPrefs.DeleteAll();
    }

    [ContextMenu("ResetRanking")]
    public void ResetRanking()
    {
        Ranking.ResetRanking();
    }

    [ContextMenu("Set")]
    public void Set()
    {
        Ranking.ResetRanking();
        data.ranking.ForEach(x => Ranking.AddRanking(x));
    }
}
