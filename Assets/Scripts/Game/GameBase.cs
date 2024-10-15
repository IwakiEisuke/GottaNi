using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class GameBase : MonoBehaviour
{
    public Action<GameSectionResult> endGame;
    public GameSectionResult result;

    public abstract void StartGame();

    public virtual void OnComplete()
    {
        endGame(result);
    }
}
