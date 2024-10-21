using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ゲームセクションの結果の保持し、ゲーム進行を制御するクラス。
/// </summary>
public class GameSectionManager : ResultSender
{
    [SerializeField] GameSpawnerBase spawner;
    [SerializeField] float spacing;
    [SerializeField] float offsetY;
    [SerializeField] float startDelay;
    [SerializeField] float endDelay;

    GameSectionResult init = new(0, 0, true, 0);
    GameSectionResult result = new(0, 0, true, 0);
    GameBase[] sequence;
    int count = -1;

    /// <summary>
    /// ゲームシーケンスを順次実行する。
    /// </summary>
    /// <param name="result"></param>
    private void RunSequence(GameSectionResult result)
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
                StartCoroutine(EndGame(startDelay, endDelay)); // シーケンスを完走したらオブザーバーへ通知。
            }
        }
        else //失敗したらその時点で終了する
        {
            StartCoroutine(EndGame(startDelay, endDelay));
        }
    }

    /// <summary>
    /// 新たなゲームシーケンスを開始する
    /// </summary>
    [ContextMenu("StartSection")]
    public void StartSection()
    {
        var newSequence = spawner.CreateSequence();

        foreach (var game in newSequence)
        {
            game.transform.parent = transform;
        }

        for (int i = 0; i < newSequence.Length; i++)
        {
            var vec = new Vector3();
            // 等間隔で横並びにする
            vec.x = (i - ((newSequence.Length - 1) / 2f)) * spacing;
            // 縦にずらす
            vec.y = offsetY;

            newSequence[i].transform.position += vec;
        }

        sequence = newSequence;
        count = -1;
        result = init;
        RunSequence(result);
    }

    /// <summary>
    /// 各ゲームを終了させる
    /// </summary>
    private IEnumerator EndGame(float startDelay, float endDelay)
    {
        yield return new WaitForSeconds(startDelay);

        foreach (var game in sequence)
        {
            if (game.gameObject.activeSelf)
            {
                game.PlayClosingAnimation();
            }
        }

        yield return new WaitForSeconds(endDelay);

        ChangeState(this.result); // ChangeState実行時点で次のセクションが実行されることに注意
    }

    /// <summary>
    /// イベント等で外から呼び出す用
    /// </summary>
    public void ExecuteEndGame()
    {
        StartCoroutine(EndGame(startDelay, endDelay));
    }
}
