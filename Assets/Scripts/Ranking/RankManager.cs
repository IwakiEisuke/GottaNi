using System.Linq;
using UnityEngine;

/// <summary>
/// �X�R�A�ɉ����������N��\������N���X
/// </summary>
public class RankManager : MonoBehaviour
{
    [SerializeField] Transform parent;
    [SerializeField] int resultScore;
    [SerializeField] float delay;
    [SerializeField] Rank[] ranks;

    void Start()
    {
        UpdateCurrentScore();
        Invoke(nameof(ShowRank), delay);
    }

    void UpdateCurrentScore()
    {
        var sm = FindObjectOfType<ScoreManager>();
        if (sm)
        {
            resultScore = sm.GetScore();
        }
    }

    public void ShowRank()
    {
        var rank = Rank.GetRank(ranks, resultScore);
        if (rank != null)
        {
            Instantiate(rank.rankImage, parent);
        }
    }
}

[System.Serializable]
class Rank
{
    public int scoreAbove;
    public GameObject rankImage;

    public static Rank GetRank(Rank[] ranks, int currentScore)
    {
        // �͈͊O�̏ꍇ��ԒႢ�����N��Ԃ�
        if (currentScore < ranks.Min(x => x.scoreAbove))
        {
            return ranks.OrderBy(x => x.scoreAbove).First();
        }

        var currentRank = ranks.OrderByDescending(x => x.scoreAbove).First(x => x.scoreAbove <= currentScore);
        return currentRank;
    }
}