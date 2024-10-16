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

    private void Start()
    {
        timeManager.Init();
        Invoke(nameof(StartGame), timeToStart);
    }

    public void StartGame()
    {
        section = spawner.StartNewSection();

        section.RegisterObserver(scoreManager);
        section.RegisterObserver(gauge);
        section.RegisterObserver(this);

        timeManager.StartTimer();
    }

    public void EndGame()
    {
        Ranking.AddRanking(scoreManager.GetScore());
    }

    public void OnSectionComplete(GameSectionResult result)
    {
        gameCount++;
        StartGame();
    }
}