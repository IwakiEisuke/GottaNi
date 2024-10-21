using Unity.VisualScripting;
using UnityEngine;

public class PinLockGameManager : MonoBehaviour, IGameSectionResultObserver
{
    [SerializeField] float timeToStart;
    [SerializeField] int gameCount;
    [SerializeField] GameSectionManager section;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ChanceGauge gauge;
    [SerializeField] TimeManager timeManager;

    bool isPlaying = true;

    private void Start()
    {
        Invoke(nameof(StartGame), timeToStart);
    }

    private void StartGame()
    {
        timeManager.Init();

        section.RegisterObserver(scoreManager);
        section.RegisterObserver(gauge);
        section.RegisterObserver(timeManager);
        section.RegisterObserver(this);

        timeManager.StartTimer();

        Next();
    }

    private void Next()
    {
        if (isPlaying)
        {
            section.StartSection();
        }
    }

    private void EndGame()
    {
        Ranking.AddRanking(scoreManager.GetScore());
        isPlaying = false;
    }

    public void OnSectionComplete(GameSectionResult result)
    {
        gameCount++;
        Next();
    }
}