using System.Collections.Generic;
using UnityEngine;

public static class Ranking
{
    readonly static RankingData data = GetRanking();

    public static RankingData GetRanking()
    {
        var json = PlayerPrefs.GetString("ranking");
        return JsonUtility.FromJson(json, typeof(RankingData)) as RankingData;
    }
    public static void AddRanking(int item)
    {
        data.ranking.Add(item);
        SetRanking();
    }

    public static void ResetRanking()
    {
        var json = JsonUtility.ToJson(new RankingData());
        PlayerPrefs.SetString("ranking", json);
    }
    static void SetRanking()
    {
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("ranking", json);
    }
}

[System.Serializable]
public class RankingData
{
    public List<int> ranking;
}
