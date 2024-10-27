using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 各ゲームの親クラス（多態性用。interfaceでいいかも）
/// </summary>
public abstract class GameBase : MonoBehaviour
{
    public Action<GameSectionResult> sendResult;
    protected GameSectionResult result;
    protected bool isPlaying;
    protected bool gameClosed;

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
    public virtual void CompleteGame()
    {
        isPlaying = false;
        SendResult();
    }

    /// <summary>
    /// ゲームセクション完了時、またはタイムオーバー時に実行する関数
    /// </summary>
    public virtual void PlayClosingAnimation()
    {
        isPlaying = false;
        gameObject.SetActive(false);
    }
}
