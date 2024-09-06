using UnityEngine;

public class PinLockGameManager : MonoBehaviour
{
    [SerializeField] int gameCount;
    [SerializeField] int score;
    [SerializeField] PinLockController[] sections;
    [SerializeField] PinLockProperties[] adds;
    PinLockController game;

    void Awake()
    {
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
    }
}
