using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameSectionResultObserver
{
    void OnSectionComplete(GameSectionResult result);
}

/// <summary>
/// 各ゲームセクションの結果を通知するSubjectが継承するクラス
/// </summary>
public abstract class ResultSender : MonoBehaviour
{
    private readonly List<IGameSectionResultObserver> observers = new();

    public virtual void ChangeState(GameSectionResult result) => NotifyObservers(result);

    public void NotifyObservers(GameSectionResult result)
    {
        observers.ForEach(x => x.OnSectionComplete(result));
    }

    public void RegisterObserver(IGameSectionResultObserver observer)
    {
        observers.Add(observer);
    }
}

public struct GameSectionResult
{
    public int score;
    public float chancePoint;
    public bool success;
    public int time;

    public GameSectionResult(int score, float chancePoint, bool success, int time)
    {
        this.score = score;
        this.chancePoint = chancePoint;
        this.success = success;
        this.time = time;
    }

    public static GameSectionResult operator +(GameSectionResult a, GameSectionResult b)
    {
        return new GameSectionResult(a.score + b.score, a.chancePoint + b.chancePoint, a.success && b.success, a.time + b.time);
    }
}
