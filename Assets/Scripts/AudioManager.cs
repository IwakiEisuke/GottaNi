using DG.Tweening;
using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager incetance;
    [SerializeField]
    AudioClip MatchingSuccess,
              MatchingFailure,
              ButtonHover,
              ButtonPressed,
              GameOpen,
              GameClose,
              OpenResult,
              CloseResult,
              GaugeFull,
              AddScore;

    [SerializeField] AudioSource SESource;
    [SerializeField] AudioSource BGMSource, InGameBGM,
              OutGameBGM;

    [SerializeField] float transitionTime = 5f;


    private void Awake()
    {
        if (incetance != null)
        {
            Destroy(incetance);
        }
        incetance = this;
    }

    public static void Play(SoundType type)
    {
        if (incetance)
        {
            var se = GetSound(type);
            if (se != null)
            {
                incetance.SESource.PlayOneShot(se);
            }
        }
    }

    public static void PlayBGM(BGMType type, float volume)
    {
        var transitionTime = incetance.transitionTime;
        if (incetance)
        {
            var bgm = GetBGM(type);

            if (!bgm.isPlaying)
            {
                ChangeBGMVolume(bgm, volume, transitionTime);
                bgm.Play();

                var types = Enum.GetValues(typeof(BGMType));
                foreach (BGMType t in types)
                {
                    if (t != type)
                    {
                        var otherBGM = GetBGM(t);
                        ChangeBGMVolume(otherBGM, 0, transitionTime);
                        otherBGM.Stop();
                    }
                }
            }
        }
    }

    public static Tweener ChangeBGMVolume(AudioSource source, float volume, float duration)
    {
        return DOTween.To(() => source.volume, x => source.volume = x, volume, duration);
    }

    static AudioClip GetSound(SoundType type)
    {
        switch (type)
        {
            case SoundType.MatchingSuccess: return incetance.MatchingSuccess;
            case SoundType.MatchingFailure: return incetance.MatchingFailure;
            case SoundType.ButtonHover: return incetance.ButtonHover;
            case SoundType.ButtonPressed: return incetance.ButtonPressed;
            case SoundType.OpenGame: return incetance.GameOpen;
            case SoundType.CloseGame: return incetance.GameClose;
            case SoundType.OpenResult: return incetance.OpenResult;
            case SoundType.CloseResult: return incetance.CloseResult;
            case SoundType.GaugeFull: return incetance.GaugeFull;
            case SoundType.AddScore: return incetance.AddScore;
            
            default: return null;
        }
    }

    static AudioSource GetBGM(BGMType type)
    {
        switch (type)
        {
            case BGMType.InGameBGM: return incetance.InGameBGM;
            case BGMType.OutGameBGM: return incetance.OutGameBGM;
            default: return null;
        }
    }
}

public enum SoundType
{
    MatchingSuccess,
    MatchingFailure,
    ButtonHover,
    ButtonPressed,
    OpenGame,
    CloseGame,
    OpenResult,
    CloseResult,
    GaugeFull,
    AddScore,
}

public enum BGMType
{
    InGameBGM,
    OutGameBGM,
}
