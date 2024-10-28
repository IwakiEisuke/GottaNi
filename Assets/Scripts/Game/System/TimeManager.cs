using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour, IGameSectionResultObserver
{
    [SerializeField] float animateDuration;
    [SerializeField] int timeLimit;
    [SerializeField] Text timerText;
    [SerializeField] string format = @"mm\:ss";

    [Space(16)]
    [SerializeField] UnityEvent onTimeUpEvent;
    float t;
    float dummyTime;

    public bool IsTimeUp { get; private set; }

    bool isPlaying;

    float beforeAnimatedTime;
    bool isTweening;

    private void Start()
    {
        t = timeLimit;
        dummyTime = t;
    }

    public void Init()
    {
        t = timeLimit;
        dummyTime = t;
        var span = TimeSpan.FromSeconds(Mathf.CeilToInt(t));
        timerText.text = span.ToString(format);
    }

    public void StartTimer()
    {
        isPlaying = true;
    }

    void Update()
    {
        if (isPlaying) t -= Time.deltaTime;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (!IsTimeUp)
        {
            if(!isTweening) dummyTime -= Time.deltaTime;
            var span = TimeSpan.FromSeconds(Mathf.CeilToInt(dummyTime));
            timerText.text = span.ToString(format);

            if (t <= 0)
            {
                EndGame();
            }
        }
    }

    int previousTime;
    private void AddTime(float add)
    {
        t += add;
        PlayTimeAnimation();
    }

    private void PlayTimeAnimation()
    {
        isTweening = true;

        void Setter(float x)
        {
            dummyTime = x;
            if (!PinLockGameManager.GameOver) // ゲームオーバー後に音を鳴らさない
            {
                if (previousTime != (int)x) AudioManager.Play(SoundType.AddScore);
                previousTime = (int)x;
            }
        }

        DOTween.To(() => dummyTime, x => Setter(x), t - animateDuration, animateDuration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isTweening = false;
                dummyTime = t;
            });
    }

    [ContextMenu("Force quit the game")]
    void EndGame()
    {
        t = 0;
        isPlaying = false;
        onTimeUpEvent.Invoke();
        IsTimeUp = true;
    }

    public void ViewResult()
    {
        SceneManager.LoadScene("Result", LoadSceneMode.Additive);
    }

    public void OnSectionComplete(GameSectionResult result)
    {
        AddTime(result.time);
    }
}