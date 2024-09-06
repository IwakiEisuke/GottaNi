using UnityEngine;

public class PinLockGameManager : MonoBehaviour
{
    [SerializeField] int gameCount;
    [SerializeField] GameObject pinLockGamePref;
    [SerializeField] int score;


    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        gameCount++;
        var game = Instantiate(pinLockGamePref, transform).GetComponent<PinLockController>();
        game.OnCompleteAction += StartGame;
    }
}
