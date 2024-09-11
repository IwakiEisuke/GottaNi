using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingDebugger : MonoBehaviour
{
    [SerializeField] RankingData data;

    public void Delete()
    {
        PlayerPrefs.DeleteAll();
    }
    public void ResetRanking()
    {
        Ranking.ResetRanking();
    }

    public void Set()
    {
        Ranking.ResetRanking();
        data.ranking.ForEach(x => Ranking.AddRanking(x));
    }
}
