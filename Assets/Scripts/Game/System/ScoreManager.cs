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
    [SerializeField] float soundInterval;
    [SerializeField] AnimationCurve durationCurve;
    [SerializeField] AudioSource addScoreAS;
    int previousScore;
    float prevPlaySoundTime;

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
        prevPlaySoundTime = 0; // 最初は鳴らす

        void Setter(int x)
        {
            scoreText.text = preText + x.ToString(format);
            if (!PinLockGameManager.GameOver && previousScore != x && soundInterval < Time.time - prevPlaySoundTime)
            {
                //WebGLビルドの音声圧縮の影響でクリックノイズが発生するため、0.1秒スキップしたところから再生する。
                    //短期間での連続再生時のノイズは治らなかったが0.03秒インターバルを挟んだ状態での連続再生時のノイズは低減した
                addScoreAS.Stop(); // 再生中に timeSamples を変えてもうまく動かない
                addScoreAS.timeSamples = 4410; // サンプル数で再生位置を指定。SEのサンプリング周波数 44100Hz / 10s => 開幕0.1秒をスキップ
                addScoreAS.Play();

                previousScore = x;
                prevPlaySoundTime = Time.time;
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
