using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] SoundType soundType;
    public void Play()
    {
        AudioManager.Play(soundType);
    }
}
