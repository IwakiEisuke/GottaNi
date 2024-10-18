using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 各ゲームの親クラス（多態性用。interfaceでいいかも）
/// </summary>
public abstract class GameBase : MonoBehaviour
{
    public Action<GameSectionResult> sendResult;
    public GameSectionResult result;
    public bool isPlaying;

    public abstract void StartGame();

    /// <summary>
    /// 結果を通知。ゲームシーケンスを進ませる。
    /// </summary>
    public void SendResult()
    {
        sendResult?.Invoke(result);
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    public virtual void EndGame()
    {
        gameObject.SetActive(false);
        SendResult();
    }
}
