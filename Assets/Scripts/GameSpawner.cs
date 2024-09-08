using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawner : MonoBehaviour
{
    [SerializeField] PinLockController normalGame;
    [SerializeField] ChanceGauge chanceGauge;

    public PinLockController CreateGame()
    {
        return Instantiate(normalGame.gameObject, transform).GetComponent<PinLockController>();
    }
}
