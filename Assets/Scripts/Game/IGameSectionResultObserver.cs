using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameSectionResultObserver
{
    void OnSectionComplete(GameSectionResult result);
}


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

    public GameSectionResult(int score, float chancePoint, bool success)
    {
        this.score = score;
        this.chancePoint = chancePoint;
        this.success = success;
    }
}
