using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour, IGameSectionResultObserver
{
    [SerializeField] string preText = "Score : ";
    [SerializeField] string format = "D3"; 

    [Space(16)]
    [SerializeField] Text scoreText;
    [SerializeField] int score;
    [SerializeField] float tweenDuration;
    [SerializeField] float seMinDelay;
    int previousScore;
    float seTime;

    private void Start()
    {
        scoreText.text = preText + score.ToString(format);
    }

    private void AddScore(int add)
    {
        var dummy = score;

        DOTween.To
            (
            () => dummy,
            x => {
                scoreText.text = preText + x.ToString(format);
                if (!PinLockGameManager.GameOver)
                {
                    if (previousScore != x && seTime > seMinDelay)
                    {
                        AudioManager.Play(SoundType.AddScore);
                        seTime = 0;
                    }
                    previousScore = x;
                    seTime += Time.deltaTime;
                }
                 }, 
            score + add, 
            tweenDuration
            );

        score += add;
    }

    public int GetScore() { return score; }

    public void OnSectionComplete(GameSectionResult result)
    {
        AddScore(result.score);
    }
}
