using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームシーケンスを作成する
/// </summary>
public class GameSpawnManager : GameSpawnerBase
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

    /// <summary>
    /// 新たにゲームシーケンスを作成する
    /// </summary>
    public override GameBase[] CreateSequence()
    {
        List<GameBase> sequence = new();

        if (chanceGauge.IsChance)
        {
            sequence.Add(Random.Range(0, 100f) switch
            {
                < 50 => CreateGame(_2xGame),
                < 75 => CreateGame(_5xGame),
                _ => CreateGame(_10xGame),
            });

            if (Random.value > 0.5f)
            {
                sequence.Add(CreateGame(timeBonusGame));
            }
        }
        else
        {
            sequence.Add(CreateGame(normalGame));
        }
        return sequence.ToArray();
    }
}

public abstract class GameSpawnerBase : MonoBehaviour
{
    public abstract GameBase[] CreateSequence();

    protected virtual GameBase CreateGame(GameBase go)
    {
        return Instantiate(go).GetComponent<GameBase>();
    }
}