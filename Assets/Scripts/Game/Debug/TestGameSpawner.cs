using System.Collections.Generic;
using UnityEngine;

public class TestGameSpawner : GameSpawnerBase, IGameSectionResultObserver
{
    [SerializeField] GameSectionManager section;
    [SerializeField] GameBase[] sequence;
    [SerializeField] bool playOnAwake;

    private void Start()
    {
        section.RegisterObserver(this);
        if (playOnAwake)
        {
            Run();
        }
    }

    private void Run()
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

    public void OnSectionComplete(GameSectionResult result)
    {
        Run();
    }
}
