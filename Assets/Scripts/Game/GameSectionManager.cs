using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ゲームセクションの結果の保持し、ゲーム進行を制御するクラス。
/// </summary>
public class GameSectionManager : ResultSender
{
    GameSectionResult init = new(0, 0, true, 0);
    GameSectionResult result = new(0, 0, true, 0);
    GameBase[] sequence;
    int count = -1;

    /// <summary>
    /// ゲームシーケンスを順次実行する。
    /// </summary>
    /// <param name="result"></param>
    void RunSequence(GameSectionResult result)
    {
        this.result += result;

        if (this.result.success) // 前にプレイされたゲームが成功したか？
        {
            count++;
            if (count < sequence.Length)
            {
                sequence[count].StartGame();
                sequence[count].sendResult = RunSequence; // ゲームがプレイされた後、ゲーム側でこのメソッドを呼び出す
            }
            else
            {
                ChangeState(this.result); // シーケンスを完走したらオブザーバーへ通知。
            }
        }
        else //失敗したらその時点で終了する
        {
            EndGame();
            ChangeState(this.result); // ChangeState実行時点で次のセクションが実行されることに注意
        }
    }

    /// <summary>
    /// 渡されたゲームシーケンスを開始する
    /// </summary>
    /// <param name="newSequence"></param>
    public void StartSection(GameBase[] newSequence)
    {
        sequence = newSequence;
        count = -1;
        result = init;
        RunSequence(result);
    }

    /// <summary>
    /// 各ゲームを終了させる
    /// </summary>
    public void EndGame()
    {
        foreach (var game in sequence)
        {
            if (game.gameObject.activeSelf)
            {
                game.EndGame();
            }
        }
    }
}
