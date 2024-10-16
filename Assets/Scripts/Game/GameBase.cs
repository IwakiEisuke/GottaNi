using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class GameBase : MonoBehaviour
{
    public Action<GameSectionResult> sendResult;
    public GameSectionResult result;

    public abstract void StartGame();

    public virtual void OnComplete()
    {
        sendResult(result);
    }

    public virtual void ExitGame()
    {
        gameObject.SetActive(false);
    }
}
