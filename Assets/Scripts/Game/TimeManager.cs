using DG.Tweening;
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

    public bool IsTimeUp { get; private set; }

    bool isPlaying;

    private void Start()
    {
        t = timeLimit;
    }

    public void Init()
    {
        t = timeLimit;
        var span = TimeSpan.FromSeconds(Mathf.CeilToInt(t));
        timerText.text = span.ToString(format);
    }

    public void StartTimer()
    {
        isPlaying = true;
    }

    void Update()
    {
        Debug.Log(DOTween.IsTweening(gameObject));

        if (isPlaying) t -= Time.deltaTime;

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (!IsTimeUp)
        {
            var span = TimeSpan.FromSeconds(Mathf.CeilToInt(t));
            timerText.text = span.ToString(format);

            if (t <= 0)
            {
                EndGame();
            }
        }
    }

    private void AddTime(float add)
    {
        DOTween.To(() => t, x => t = x, t + add, animateDuration).SetEase(Ease.Linear);
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