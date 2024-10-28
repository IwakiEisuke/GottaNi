using UnityEngine;
using UnityEngine.UI;

public class AddScoreDisplayer : MonoBehaviour, IGameSectionResultObserver
{
    [SerializeField] Text scoreText;
    [SerializeField] Text magText;

    public void Display(int baseScore, float mag)
    {
        DisplayScore(baseScore);
        DisplayMagnification(mag);
    }

    void DisplayScore(int score)
    {
        Debug.Log("DisplayScore : " + score);
    }

    void DisplayMagnification(float mag)
    {
        Debug.Log("DisplayMag : " + mag);
    }

    void IGameSectionResultObserver.OnUpdateResult(GameSectionResult result)
    {
        DisplayScore(result.score);
        //DisplayMagnification(result.mag);
    }

    public void OnSectionComplete(GameSectionResult result)
    {

    }
}
