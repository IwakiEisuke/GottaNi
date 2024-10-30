using DG.Tweening;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
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
              AddScore,
              AddTime,
              TimeOpen,
              TimeStart,
              TimeClose,
              TimeSuccess,
              TimeFailure,
              NewRecord,
              RankLow,
              RankMid,
              RankHigh,
              RankVeryHigh;

    [SerializeField] AudioSource SESource;
    [SerializeField] AudioSource BGMSource, InGameBGM, OutGameBGM;

    [SerializeField] float transitionTime = 5f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static void Play(SoundType type)
    {
        if (instance)
        {
            var se = GetSound(type);
            if (se != null)
            {
                instance.SESource.PlayOneShot(se);
            }
        }
    }

    public static void PlayBGM(BGMType type, float volume)
    {
        var transitionTime = instance.transitionTime;
        if (instance)
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
        return type switch
        {
            SoundType.MatchingSuccess => instance.MatchingSuccess,
            SoundType.MatchingFailure => instance.MatchingFailure,
            SoundType.ButtonHover => instance.ButtonHover,
            SoundType.ButtonPressed => instance.ButtonPressed,
            SoundType.OpenGame => instance.GameOpen,
            SoundType.CloseGame => instance.GameClose,
            SoundType.OpenResult => instance.OpenResult,
            SoundType.CloseResult => instance.CloseResult,
            SoundType.GaugeFull => instance.GaugeFull,
            SoundType.AddScore => instance.AddScore,
            SoundType.AddTime => instance.AddTime,
            SoundType.TimeOpen => instance.TimeOpen,
            SoundType.TimeStart => instance.TimeStart,
            SoundType.TimeClose => instance.TimeClose,
            SoundType.TimeSuccess => instance.TimeSuccess,
            SoundType.TimeFailure => instance.TimeFailure,
            SoundType.NewRecord => instance.NewRecord,
            SoundType.RankLow => instance.RankLow,
            SoundType.RankMid => instance.RankMid,
            SoundType.RankHigh => instance.RankHigh,
            SoundType.RankVeryHigh => instance.RankVeryHigh,
            _ => null,
        };
    }

    static AudioSource GetBGM(BGMType type)
    {
        return type switch
        {
            BGMType.InGameBGM => instance.InGameBGM,
            BGMType.OutGameBGM => instance.OutGameBGM,
            _ => null,
        };
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
    AddTime,
    TimeOpen,
    TimeStart,
    TimeClose,
    TimeSuccess,
    TimeFailure,
    NewRecord,
    RankLow,
    RankMid,
    RankHigh,
    RankVeryHigh,
}

public enum BGMType
{
    InGameBGM,
    OutGameBGM,
}
