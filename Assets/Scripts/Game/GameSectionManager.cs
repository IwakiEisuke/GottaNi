using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSectionManager : ResultSender
{
    GameSectionResult result = new();
    GameBase[] sequence;
    int count = 0;

    void RunNextGame(GameSectionResult result)
    {
        if (result.success)
        {
            if (count < sequence.Length)
            {
                sequence[count].StartGame();
                sequence[count].sendResult = RunNextGame;
                this.result += result;
                count++;
            }
            else
            {
                ChangeState(result);
            }
        }
        else //Ž¸”s‚µ‚½‚ç‚»‚ÌŽž“_‚ÅI—¹‚·‚é
        {
            ChangeState(result);
        }
    }

    public void SetGames(GameBase[] game)
    {
        sequence = game;
        RunNextGame(result);
    }
}
