using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PinLockGameManager : MonoBehaviour
{
    [SerializeField] int gameCount;
    [SerializeField] GameObject pinLockGamePref;
    

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
