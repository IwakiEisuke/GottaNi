using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] string preText = "Score : ";
    [SerializeField] string format = "D3"; 

    [Space(16)]
    [SerializeField] Text scoreText;
    [SerializeField] int score;

    private void Start()
    {
        scoreText.text = preText + score.ToString(format);
    }

    public void AddScore(int add)
    {
        score += add;
        scoreText.text = preText + score.ToString(format);
    }
}
