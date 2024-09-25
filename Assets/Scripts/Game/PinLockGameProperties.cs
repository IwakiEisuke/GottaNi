using UnityEngine;

public class PinLockGameProperties : MonoBehaviour
{
    [Header("GameSettings")]
    /// <summary>�����̃s���̐�</summary>
    public int locksCount;
    /// <summary>�����̃s���̐�</summary>
    public int keysCount;
    /// <summary>�ő�̃s���̒���</summary>
    public int maxLength;
    /// <summary>�ŏ��̃s���̒���</summary>
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

    /// <summary>���s���ɃQ�[�����I�������邩</summary>
    [Header("Others")]
    public bool gameCloseEvenIfMissing;
}