using UnityEngine;

public class PinLockGameManager : MonoBehaviour
{
    [SerializeField] int gameCount;
    [SerializeField] PinLockRandomizer[] adds;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ChanceGauge gauge;
    [SerializeField] GameSpawner spawner;
    PinLockController game;

    void Awake()
    {
        StartGame();
    }

    void StartGame()
    {
        game = spawner.CreateGame();
        adds[gameCount % adds.Length].Add(game);
        game.OnCompleteAction += OnCompleteAction;
        game.OnCompleteAction += StartGame;
    }

    void OnCompleteAction()
    {
        gameCount++;
        scoreManager.AddScore(game.AddScore);
        gauge.AddChancePoint(game.AddChancePoint);
    }
}