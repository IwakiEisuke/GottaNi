using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] float waitTime;

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
}
