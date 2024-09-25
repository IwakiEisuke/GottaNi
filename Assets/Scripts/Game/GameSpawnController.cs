using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawnController : MonoBehaviour
{
    [Header("ScoreGames")]
    [SerializeField] PinLockController normalGame;
    [SerializeField] PinLockController _2xGame;
    [SerializeField] PinLockController _5xGame;
    [SerializeField] PinLockController _10xGame;

    [Header("AdditionalGames")]
    [SerializeField] TimeBonusGame timeBonusGame;

    [Header("Gauge")]
    [SerializeField] ChanceGauge chanceGauge;

    [Header("Position")]
    [SerializeField] float centerX;
    [SerializeField] float centerY;

    public PinLockController CreateGame()
    {
        if (chanceGauge.IsChance)
        {
            return Random.Range(0, 100f) switch
            {
                < 50 => Create(_2xGame),
                < 75 => Create(_5xGame),
                _ => Create(_10xGame),
            };
        }
        else
        {
            return Create(normalGame);
        }
    }

    PinLockController Create(PinLockController go)
    {
        var game = Instantiate(go, transform).GetComponent<PinLockController>();

        return game;
    }
}
