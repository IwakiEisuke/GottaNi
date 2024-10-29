using UnityEngine;
using UnityEngine.UI;

public class CurrentResultDisplayer : MonoBehaviour, IGameSectionResultObserver
{
    [SerializeField] Text scoreText;
    [SerializeField] Text timeText;

    [SerializeField] Animator[] anims;

    private void Start()
    {
        scoreText.text = "";
        timeText.text = "";
    }

    void DisplayNum(Text text, int add)
    {
        if (add != 0)
            text.text = $"+{add}";
        else
            text.text = "";
    }

    void IGameSectionResultObserver.OnUpdateResult(GameSectionResult result)
    {
        DisplayNum(scoreText, result.score);
        DisplayNum(timeText, result.time);

        foreach (var a in anims)
        {
            a.SetTrigger("Open");
        }
    }

    public void OnSectionComplete(GameSectionResult result)
    {
        foreach (var a in anims)
        {
            a.SetTrigger("Close");
        }
    }
}
