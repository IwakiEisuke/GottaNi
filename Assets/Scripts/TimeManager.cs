using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] int timeLimit;
    [SerializeField] Text timerText;
    [SerializeField] string format = @"mm\:ss";
    float t;

    public bool IsTimeUp { get; private set; }

    void Start()
    {
        t = timeLimit;
    }

    void Update()
    {
        AddTime(-Time.deltaTime);
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
                IsTimeUp = true;
            }
        }
    }
}
