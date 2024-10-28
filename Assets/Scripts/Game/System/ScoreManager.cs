using DG.Tweening;
using UnityEngine;
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

    int beforeAnimatedScore;

    private void Start()
    {
        scoreText.text = preText + score.ToString(format);
    }

    private void AddScore(int add)
    {
        score += add;
        PlayScoreAnimation();
    }

    private void PlayScoreAnimation()
    {
        var dummy = beforeAnimatedScore;
        beforeAnimatedScore = score;

        void Setter(int x)
        {
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
        }

        DOTween.To(() => dummy, x => Setter(x), score, tweenDuration);
    }

    public int GetScore() { return score; }

    public void OnSectionComplete(GameSectionResult result)
    {
        AddScore(result.score);
    }
}
