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
    [SerializeField] float maxTweenDuration;
    [SerializeField] float maxTweenScore;
    [SerializeField] AnimationCurve durationCurve;
    [SerializeField] AudioSource addScoreAS;
    int previousScore;
    float seTime;

    int beforeAnimatedScore;

    private void Start()
    {
        addScoreAS.clip = AudioManager.GetSound(SoundType.AddScore);
        scoreText.text = preText + score.ToString(format);
    }

    private void AddScore(int add)
    {
        score += add;
    }

    private void PlayScoreAnimation()
    {
        var dummy = beforeAnimatedScore;
        beforeAnimatedScore = score;

        void Setter(int x)
        {
            scoreText.text = preText + x.ToString(format);
            if (!PinLockGameManager.GameOver && previousScore != x)
            {
                addScoreAS.Play();
                previousScore = x;
            }
        }

        var durationT = (score - previousScore) / maxTweenScore;
        var tweenDuration = durationCurve.Evaluate(durationT);
        DOTween.To(() => dummy, x => Setter(x), score, tweenDuration).SetEase(Ease.InOutCirc);
    }

    void IGameSectionResultObserver.OnUpdateResult(GameSectionResult result)
    {
        AddScore(result.score);
    }

    public int GetScore() { return score; }

    public void OnSectionComplete(GameSectionResult result)
    {
        if (result.score != 0)
            PlayScoreAnimation();
    }
}
