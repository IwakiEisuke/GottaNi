using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddScoreDisplayer : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Text magText;
    public void Display(int baseScore, float mag)
    {
        DisplayScore(baseScore);
        DisplayMag(mag);
    }

    void DisplayScore(int score)
    {

    }

    void DisplayMag(float mag)
    {

    }
}
