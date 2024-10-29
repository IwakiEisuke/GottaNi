using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AddScoreDisplayer : MonoBehaviour, IGameSectionResultObserver
{
    [SerializeField] Text scoreText;
    [SerializeField] Text magText;

    [SerializeField] Animator[] anims;

    private void Start()
    {
        scoreText.text = "";
        //magText.text = "";
    }

    public void Display(int baseScore, float mag)
    {
        DisplayScore(baseScore);
        DisplayMagnification(mag);
    }

    void DisplayScore(int score)
    {
        Debug.Log("DisplayScore : " + score);
        scoreText.text = $"+{score}";
    }

    void DisplayMagnification(float mag)
    {
        Debug.Log("DisplayMag : " + mag);
        magText.text = $"x{mag}";
    }

    void IGameSectionResultObserver.OnUpdateResult(GameSectionResult result)
    {
        DisplayScore(result.score);
        //DisplayMagnification(result.mag);

        foreach (var a in anims)
        {
            a.SetTrigger("Open");
        }
    }

    public void OnSectionComplete(GameSectionResult result)
    {
        foreach(var a in anims)
        {
            a.SetTrigger("Close");
        }
    }
}
