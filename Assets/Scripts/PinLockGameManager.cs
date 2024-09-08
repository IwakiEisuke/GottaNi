using UnityEngine;

public class PinLockGameManager : MonoBehaviour
{
    [SerializeField] int gameCount;
    [SerializeField] PinLockRandomizer[] adds;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ChanceGauge gauge;
    [SerializeField] GameSpawner spawner;
    [SerializeField] TimeManager timeManager;
    PinLockController game;
    bool endGame;

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
        if (gauge.IsChance) gauge.ResetChance();

        gameCount++;
        scoreManager.AddScore(game.AddScore);
        gauge.AddChancePoint(game.AddChancePoint);
    }

    private void Update()
    {
        if (timeManager.IsTimeUp && !endGame)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        endGame = true;
        Debug.Log("GameOver");
    }
}