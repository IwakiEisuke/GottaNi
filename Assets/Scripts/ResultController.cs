using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] float waitTime;
    [SerializeField] Button titleButton, restartButton;

    private void Start()
    {
        Invoke(nameof(Open), waitTime);
    }

    public void Open()
    {
        AudioManager.Play(SoundType.OpenResult);
    }

    public void Close()
    {
        AudioManager.Play(SoundType.CloseResult);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            titleButton.onClick.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            restartButton.onClick.Invoke();
        }
    }
}
