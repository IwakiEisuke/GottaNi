using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _seSlider;
    [Header("スライダーの値を t として音量を指定します")]
    [SerializeField] AnimationCurve _volumeCurve;

    public void SetBGMVolume()
    {
        _audioMixer.SetFloat("BGM_Volume", Volume(_bgmSlider.value));

    }

    public void SetSEVolume()
    {
        _audioMixer.SetFloat("SE_Volume", Volume(_seSlider.value));
    }

    float Volume(float value)
    {
        return _volumeCurve.Evaluate(value);
    }
}
