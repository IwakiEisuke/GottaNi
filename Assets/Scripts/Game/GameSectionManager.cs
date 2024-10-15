using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSectionManager : ResultSender
{
    GameSectionResult result = new();
    List<GameBase> games = new();
    int count = 0;

    void Start()
    {
        Next(result);
    }

    void Next(GameSectionResult result)
    {
        games[count].StartGame();
        games[count].endGame = Next;
        this.result += result;
        count++;
    }
}
