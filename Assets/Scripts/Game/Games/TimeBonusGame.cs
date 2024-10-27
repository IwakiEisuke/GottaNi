using DG.Tweening;
using System.Linq;
using UnityEngine;

public class TimeBonusGame : GameBase
{
    [Header("GameSettings")]
    [SerializeField] int addTimeSeconds = 10;
    [SerializeField] float minAngleRange = 20;
    [SerializeField] float maxAngleRange = 45;
    [SerializeField] float minSpeed = 360;
    [SerializeField] float maxSpeed = 360 * 2;
    float speed;

    [Header("AnimationSettings")]
    [SerializeField] float startDuration;
    [SerializeField] float endDuration;
    [SerializeField] float successDuration;
    [SerializeField] float failureDuration;

    [Header("Others")]
    [SerializeField] Material m;
    [SerializeField] ParticleSystem[] ps;

    private void Start()
    {
        foreach (var p in ps)
        {
            var main = p.main;
            main.loop = false;
        }

        isPlaying = false;
        m.SetFloat("_HandAngle", 0);
        m.SetFloat("_Seed", Random.Range(0f, 100));

        speed = Random.Range(minSpeed, maxSpeed);
        m.SetFloat("_TargetAngle", Random.Range(0f, 360f));
        m.SetFloat("_AngleRange", Random.Range(minAngleRange, maxAngleRange));
        SetAngles();

        DOTween.To(() => 0f, x => m.SetFloat("_T", x), 1, startDuration);

        AudioManager.Play(SoundType.GaugeFull);
    }

    [ContextMenu(nameof(StartGame))]
    public override void StartGame()
    {
        isPlaying = true;
        AudioManager.Play(SoundType.OpenGame);
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        m.SetFloat("_HandAngle", m.GetFloat("_HandAngle") + speed * Time.deltaTime);
        SetAngles();

        if (Input.GetKeyDown(KeyCode.Space) && !PinLockGameManager.GameOver)
        {
            var handAngle = m.GetFloat("_HandAngle");

            var diff = handAngle - m.GetFloat("_TargetAngle");
            diff = Mathf.Abs((180 + diff) % 360 - 180);

            if (diff < m.GetFloat("_AngleRange") / 2)
            {
                result.time = addTimeSeconds;
                result.success = true;
                PlaySuccessAnimation();
            }
            else
            {
                result.time = 0;
                result.success = false;
                PlayFailureAnimation();
            }

            AudioManager.Play(result.success ? SoundType.MatchingSuccess : SoundType.MatchingFailure);
            //Debug.Log(result.success ? "success" : "failure");
            //Debug.Log($"{m.GetFloat("_HandAngle")} {m.GetFloat("_TargetAngle")} {m.GetFloat("_AngleRange")} {diff} {m.GetFloat("_AngleRange") / 2}");
        }
    }

    public override void CompleteGame()
    {
        base.CompleteGame();
    }

    private void PlaySuccessAnimation()
    {
        isPlaying = false;

        foreach (var p in ps)
        {
            p.Play();
        }

        Invoke(nameof(CompleteGame), successDuration);
    }

    private void PlayFailureAnimation()
    {
        isPlaying = false;
        Invoke(nameof(CompleteGame), failureDuration);
    }

    public override void PlayClosingAnimation()
    {
        AudioManager.Play(SoundType.CloseGame);

        DOTween.Kill(gameObject);
        DOTween.To(() => m.GetFloat("_T"), x => m.SetFloat("_T", x), 0, endDuration)
            .OnComplete(() =>
            {
                base.PlayClosingAnimation();
            });
    }

    private void SetAngles()
    {
        var targetAngle = (180 * 3 + m.GetFloat("_TargetAngle")) % 360 - 180;
        m.SetFloat("_TargetAngle", targetAngle);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetAngles();
    }
#endif

}
