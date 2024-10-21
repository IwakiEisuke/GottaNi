using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameSpawner : GameSpawnerBase
{
    [SerializeField] GameSectionManager section;
    [SerializeField] GameBase[] sequence;

    private void Start()
    {
        section.StartSection();
    }

    /// <summary>
    /// 既存のゲームシーケンスを返す
    /// </summary>
    /// <returns></returns>
    public override GameBase[] CreateSequence()
    {
        List<GameBase> newSequence = new();
        foreach (var s in sequence)
        {
            newSequence.Add(CreateGame(s));
        }
        return newSequence.ToArray();
    }
}
