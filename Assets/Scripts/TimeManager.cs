using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] int timeLimit;
    [SerializeField] Text timerText;
    [SerializeField] string format = @"mm\:ss";
    [SerializeField] UnityEvent onTimeUpEvent;
    float t;

    public bool IsTimeUp { get; private set; }
    bool isPlay;

    public void Init()
    {
        t = timeLimit;
        var span = TimeSpan.FromSeconds(Mathf.CeilToInt(t));
        timerText.text = span.ToString(format);
    }

    public void StartTimer()
    {
        isPlay = true;
    }

    void Update()
    {
        if (isPlay) AddTime(-Time.deltaTime);
    }

    public void AddTime(float add)
    {
        if (!IsTimeUp)
        {
            t += add;
            var span = TimeSpan.FromSeconds(Mathf.CeilToInt(t));
            timerText.text = span.ToString(format);

            if (t <= 0)
            {
                t = 0;
                onTimeUpEvent.Invoke();
                IsTimeUp = true;
            }
        }
    }

    public void ViewResult()
    {
        SceneManager.LoadScene("Result", LoadSceneMode.Additive);
    }
}
