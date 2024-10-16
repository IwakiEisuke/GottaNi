using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        else //Ž¸”s‚µ‚½‚ç‚»‚ÌŽž“_‚ÅI—¹‚·‚é
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
}
