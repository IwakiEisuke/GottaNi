using UnityEngine;

public class PinLockGameManager : MonoBehaviour, IGameSectionResultObserver
{
    [SerializeField] float timeToStart;
    [SerializeField] int gameCount;
    //[SerializeField] PinLockRandomizer[] adds;
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
        //adds[gameCount % adds.Length].Add(game);

        game.RegisterObserver(scoreManager);
        game.RegisterObserver(gauge);
        game.RegisterObserver(this);

        timeManager.StartTimer();
    }

    public void EndGame()
    {
        Ranking.AddRanking(scoreManager.GetScore());
        game.ExitGame();
    }

    public void OnSectionComplete(GameSectionResult result)
    {
        gameCount++;
        StartGame();
    }
}