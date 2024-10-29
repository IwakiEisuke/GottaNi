using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ゲームセクションの結果を保持し、ゲーム進行を制御するクラス。
/// </summary>
public class GameSectionManager : ResultSender
{
    [SerializeField] GameSpawnerBase spawner;
    [SerializeField] float spacing;
    [SerializeField] float offsetY;

    [SerializeField] float openDelay;
    [SerializeField] float closeStartDelay;
    [SerializeField] float closeEndDelay;

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
        if (this.result.success) // 前にプレイされたゲームが成功したか？
        {
            count++;
            if (count < sequence.Length)
            {
                sequence[count].StartGame();
                sequence[count].runNext = RunSequence; // ゲームがプレイされた後、ゲーム側でこのメソッドを呼び出す
                sequence[count].sendResult = AddResult;
            }
            else
            {
                StartCoroutine(EndSection(closeStartDelay, closeEndDelay)); // シーケンスを完走したらオブザーバーへ通知。
            }
        }
        else //失敗したらその時点で終了する
        {
            StartCoroutine(EndSection(closeStartDelay, closeEndDelay));
        }
    }

    private void AddResult(GameSectionResult result)
    {
        this.result += result;

        if (!this.result.success || count == sequence.Length - 1)
        {
            SendCurrentResult(this.result);
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
            var vec = new Vector3
            {
                // 等間隔で横並びにする
                x = (i - ((newSequence.Length - 1) / 2f)) * spacing,
                // 縦にずらす
                y = offsetY
            };

            newSequence[i].transform.position += vec;
        }

        sequence = newSequence;
        count = -1;
        result = init;
        StartCoroutine(OpenSection(openDelay));
    }

    private IEnumerator OpenSection(float delay)
    {
        yield return new WaitForSeconds(delay);

        RunSequence(result);
    }

    /// <summary>
    /// 各ゲームを終了させる
    /// </summary>
    private IEnumerator EndSection(float startDelay, float endDelay)
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
        StartCoroutine(EndSection(closeStartDelay, closeEndDelay));
    }
}
