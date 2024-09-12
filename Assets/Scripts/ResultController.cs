using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    public void Close()
    {
        AudioManager.Play(SoundType.CloseResult);
    }
}
