using UnityEngine;

public class PinLockGameManager : MonoBehaviour
{
    [SerializeField] int gameCount;
    [SerializeField] int score;
    [SerializeField] PinLockController[] sections;
    [SerializeField] PinLockProperties[] adds;


    void Awake()
    {
        StartGame();
    }

    void StartGame()
    {
        gameCount++;
        var game = Instantiate(sections[gameCount / adds.Length % sections.Length].gameObject, transform).GetComponent<PinLockController>();
        adds[(gameCount - 1) % adds.Length].Add(game);
        game.OnCompleteAction += StartGame;
    }
}
