using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawner : MonoBehaviour
{
    [SerializeField] PinLockController normalGame;
    [SerializeField] PinLockController _2xGame;
    [SerializeField] PinLockController _5xGame;
    [SerializeField] PinLockController _10xGame;
    [SerializeField] ChanceGauge chanceGauge;

    [Header("Position")]
    [SerializeField] float centerX;
    [SerializeField] float centerY;

    public PinLockController CreateGame()
    {
        if (chanceGauge.IsChance)
        {
            var rand = Random.Range(0, 100f);

            switch (rand)
            {
                case < 50:
                    return Create(_2xGame);
                case < 75:
                    return Create(_5xGame);
                default:
                    return Create(_10xGame);
            }
        }
        else
        {
            return Create(normalGame);
        }
    }

    PinLockController Create(PinLockController go)
    {
        var game = Instantiate(go, transform).GetComponent<PinLockController>();
        game.centerX = centerX;
        game.centerY = centerY;
        return game;
    }
}
