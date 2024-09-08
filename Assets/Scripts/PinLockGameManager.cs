using UnityEngine;
using UnityEngine.UI;

public class PinLockGameManager : MonoBehaviour
{
    [SerializeField] int gameCount;
    [SerializeField] int score;
    [SerializeField] PinLockController[] sections;
    [SerializeField] PinLockRandomizer[] adds;
    [SerializeField] Text scoreText;
    [SerializeField] float chanceGauge;
    [SerializeField] float maxGauge;
    [SerializeField] Slider gauge;
    PinLockController game;

    void Awake()
    {
        scoreText.text = "Score : " + score.ToString("D3");
        StartGame();
    }

    void StartGame()
    {
        game = Instantiate(sections[gameCount / adds.Length % sections.Length].gameObject, transform).GetComponent<PinLockController>();
        adds[gameCount % adds.Length].Add(game);
        game.OnCompleteAction += OnCompleteAction;
        game.OnCompleteAction += StartGame;
    }

    void OnCompleteAction()
    {
        gameCount++;
        score += game.AddScore;
        scoreText.text = "Score : " + score.ToString("D3");
        chanceGauge += game.AddGauge;
        SetGauge();
    }

    private void Update()
    {
        AddGauge(-1 * Time.deltaTime);
    }

    void AddGauge(float add)
    {
        chanceGauge += add;
        SetGauge();
    }

    void SetGauge()
    {
        gauge.value = chanceGauge / maxGauge;
    }
}
