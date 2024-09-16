using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] string preText = "Score : ";
    [SerializeField] string format = "D3"; 

    [Space(16)]
    [SerializeField] Text scoreText;
    [SerializeField] int score;
    [SerializeField] float tweenDuration;
    int previousScore;

    private void Start()
    {
        scoreText.text = preText + score.ToString(format);
    }

    public void AddScore(int add)
    {
        var dummy = score;

        DOTween.To
            (
            () => dummy,
            x => {  scoreText.text = preText + x.ToString(format);
                    if (previousScore != x) AudioManager.Play(SoundType.AddScore);
                    previousScore = x;
                 }, 
            score + add, 
            tweenDuration
            );

        score += add;
    }

    public int GetScore() { return score; }
}
