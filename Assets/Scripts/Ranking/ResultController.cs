using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] float openDelay;
    [SerializeField] Button titleButton, restartButton;
    [SerializeField] float openDuration;
    bool buttonPressed;

    private void Start()
    {
        Invoke(nameof(Open), openDelay);
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
        if (buttonPressed || openDuration > 0)
        {
            openDuration -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.T) && titleButton && titleButton.enabled)
        {
            buttonPressed = true;
            titleButton.onClick.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.R) && restartButton && restartButton.enabled)
        {
            buttonPressed = true;
            restartButton.onClick.Invoke();
        }
    }
}
