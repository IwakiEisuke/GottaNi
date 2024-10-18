using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ゲームセクションの結果の保持し、ゲーム進行を制御するクラス。
/// </summary>
public class GameSectionManager : ResultSender
{
    GameSectionResult result = new(0, 0, true, 0);
    GameBase[] sequence;
    int count = -1;

    void RunNextGame(GameSectionResult result)
    {
        if (result.success)
        {
            count++;
            if (count < sequence.Length)
            {
                sequence[count].StartGame();
                sequence[count].sendResult = RunNextGame;
                this.result += result;
            }
            else
            {
                ChangeState(result);
            }
        }
        else //失敗したらその時点で終了する
        {
            ChangeState(result);
        }
    }

    public void SetGames(GameBase[] game)
    {
        sequence = game;
        count = -1;
        RunNextGame(result);
    }

    public void EndGame()
    {
        foreach (var game in sequence)
        {
            game.EndGame();
        }
    }
}
