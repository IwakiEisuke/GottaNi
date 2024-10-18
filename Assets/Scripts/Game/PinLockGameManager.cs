using Unity.VisualScripting;
using UnityEngine;

public class PinLockGameManager : MonoBehaviour, IGameSectionResultObserver
{
    [SerializeField] float timeToStart;
    [SerializeField] int gameCount;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ChanceGauge gauge;
    [SerializeField] GameSpawnController spawner;
    [SerializeField] TimeManager timeManager;
    GameSectionManager section;

    bool isPlaying = true;

    private void Start()
    {
        Invoke(nameof(StartGame), timeToStart);
    }

    private void StartGame()
    {
        timeManager.Init();

        section = spawner.StartNewSection();

        section.RegisterObserver(scoreManager);
        section.RegisterObserver(gauge);
        section.RegisterObserver(this);

        timeManager.StartTimer();
    }

    private void Next()
    {
        if (isPlaying)
        {
            spawner.StartNewSection();
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