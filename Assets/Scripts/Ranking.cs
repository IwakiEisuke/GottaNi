using System.Collections.Generic;
using UnityEngine;

public class Ranking
{
    readonly static RankingData data = GetRanking();

    public static RankingData GetRanking()
    {
        var json = PlayerPrefs.GetString("ranking");
        return JsonUtility.FromJson(json, typeof(RankingData)) as RankingData;
    }

    static void SetRanking()
    {
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("ranking", json);
    }

    public static void AddRanking(int item)
    {
        data.ranking.Add(item);
        SetRanking();
    }
}

[System.Serializable]
public class RankingData
{
    public List<int> ranking;
}
