using DG.Tweening;
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
              AddScore,
              InGameBGM,
              OutGameBGM;

    [SerializeField] AudioSource SESource;
    [SerializeField] AudioSource BGMSource;

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
        if (incetance)
        {
            var bgm = GetBGM(type);
            if (bgm != null)
            {
                incetance.BGMSource.clip = bgm;
                incetance.BGMSource.volume = volume;
                incetance.BGMSource.Play();
            }
        }
    }

    public static void ChangeBGMVolume(float volume)
    {
        incetance.BGMSource.volume = volume;
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

    static AudioClip GetBGM(BGMType type)
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
