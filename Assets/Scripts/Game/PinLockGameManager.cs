using UnityEngine;

public class PinLockGameManager : MonoBehaviour
{
    [SerializeField] float timeToStart;
    [SerializeField] int gameCount;
    [SerializeField] PinLockRandomizer[] adds;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ChanceGauge gauge;
    [SerializeField] GameSpawnController spawner;
    [SerializeField] TimeManager timeManager;
    PinLockController game;

    private void Start()
    {
        timeManager.Init();
        Invoke(nameof(StartGame), timeToStart);
    }

    public void StartGame()
    {
        game = spawner.CreateGame();
        adds[gameCount % adds.Length].Add(game);
        game.OnCompleteAction += OnCompleteAction;
        game.OnCompleteAction += StartGame;
        timeManager.StartTimer();
    }

    void OnCompleteAction()
    {
        if (gauge.IsChance) gauge.ResetGauge();

        gameCount++;
        scoreManager.AddScore(game.AddScore);
        gauge.AddChancePoint(game.AddChancePoint);
    }

    public void EndGame()
    {
        Ranking.AddRanking(scoreManager.GetScore());
        game.ExitGame();
    }
}