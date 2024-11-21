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
        prevPlaySoundTime = 0; // �ŏ��͖炷

        void Setter(int x)
        {
            scoreText.text = preText + x.ToString(format);
            if (!PinLockGameManager.GameOver && previousScore != x && soundInterval < Time.time - prevPlaySoundTime)
            {
                //WebGL�r���h�̉������k�̉e���ŃN���b�N�m�C�Y���������邽�߁A0.1�b�X�L�b�v�����Ƃ��납��Đ�����B
                    //�Z���Ԃł̘A���Đ����̃m�C�Y�͎���Ȃ�������0.03�b�C���^�[�o�������񂾏�Ԃł̘A���Đ����̃m�C�Y�͒ጸ����
                addScoreAS.Stop(); // �Đ����� timeSamples ��ς��Ă����܂������Ȃ�
                addScoreAS.timeSamples = 4410; // �T���v�����ōĐ��ʒu���w��BSE�̃T���v�����O���g�� 44100Hz / 10s => �J��0.1�b���X�L�b�v
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
