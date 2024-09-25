using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameSectionResultObserver
{
    void OnComplete(GameSectionResult result);
}

public struct GameSectionResult
{
    public int score;
    public bool success;

    public GameSectionResult(int score, bool success)
    {
        this.score = score;
        this.success = success;
    }
}
