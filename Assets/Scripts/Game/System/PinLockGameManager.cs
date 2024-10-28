using UnityEngine;

public class PinLockGameManager : MonoBehaviour, IGameSectionResultObserver
{
    [SerializeField] float timeToStart;
    [SerializeField] int gameCount;
    [SerializeField] GameSectionManager section;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ChanceGauge gauge;
    [SerializeField] TimeManager timeManager;
    [SerializeField] AddScoreDisplayer scoreDisplayer;

    public static bool GameOver { get; private set; }
    bool isPlaying = true;

    private void Start()
    {
        GameOver = false;
        Invoke(nameof(StartGame), timeToStart);
    }

    private void StartGame()
    {
        timeManager.Init();

        section.RegisterObserver(scoreManager);
        section.RegisterObserver(gauge);
        section.RegisterObserver(timeManager);
        section.RegisterObserver(this);
        section.RegisterObserver(scoreDisplayer);

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
        GameOver = true;
        isPlaying = false;
    }

    public void OnSectionComplete(GameSectionResult result)
    {
        gameCount++;
        Next();
    }
}