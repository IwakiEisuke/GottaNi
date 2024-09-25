using UnityEngine;

public class PinLockGameProperties : MonoBehaviour
{
    [Header("GameSettings")]
    /// <summary>錠側のピンの数</summary>
    public int locksCount;
    /// <summary>鍵側のピンの数</summary>
    public int keysCount;
    /// <summary>最大のピンの長さ</summary>
    public int maxLength;
    /// <summary>最小のピンの長さ</summary>
    public int minLength;

    [Header("PinSettings")]
    public float wGap;
    public float hGap;
    public float scrollSpeed;
    public float keysPos;

    [Header("Object")]
    public GameObject lockPref;
    public GameObject keyPref;
    public Transform keyParent;

    [Header("UISettings")]
    public float uiWidth;
    public float uiHeight;
    public float openDuration;
    public SpriteRenderer frame;
    public float centerX, centerY;
    public SpriteRenderer backGround;
    public float backGroundSpeedX;
    public float backGroundSpeedY;

    [Header("GameClear AnimationSettings")]
    public float duration;
    public float strength;
    public int vibrato;

    [Header("Score Settings")]
    public int maxAddScore;
    public float multiplier;

    [Header("Chance Settings")]
    public float maxAddChancePoint;

    /// <summary>失敗時にゲームを終了させるか</summary>
    [Header("Others")]
    public bool gameCloseEvenIfMissing;
}