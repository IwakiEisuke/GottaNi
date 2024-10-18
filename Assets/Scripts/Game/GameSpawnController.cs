using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawnController : MonoBehaviour
{
    [Header("GameSectionManager")]
    [SerializeField] GameSectionManager section;

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

    public GameSectionManager StartNewSection()
    {
        section.StartSection(CreateSection());

        return section;
    }

    /// <summary>
    /// 新たにゲームシーケンスを作成する
    /// </summary>
    /// <returns></returns>
    GameBase[] CreateSection()
    {
        List<GameBase> sequence = new();

        if (!chanceGauge.IsChance)
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

    GameBase CreateGame(GameBase go)
    {
        return Instantiate(go, section.transform).GetComponent<GameBase>();
    }
}
