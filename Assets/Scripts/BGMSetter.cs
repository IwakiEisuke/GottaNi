using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSetter : MonoBehaviour
{
    [SerializeField] BGMType type;
    [SerializeField] float volume;
    void Start()
    {
        AudioManager.PlayBGM(type, volume);
    }
}
