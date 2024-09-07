using UnityEngine;
using UnityEngine.UI;

public class PinLockGameManager : MonoBehaviour
{
    [SerializeField] int gameCount;
    [SerializeField] int score;
    [SerializeField] PinLockController[] sections;
    [SerializeField] PinLockProperties[] adds;
    [SerializeField] Text scoreText;
    [SerializeField] float chanceGauge;
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
        score += game.addScore;
        scoreText.text = "Score : " + score.ToString("D3");
    }
}
