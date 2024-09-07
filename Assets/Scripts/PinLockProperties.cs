using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "new prop")]
public class PinLockProperties : ScriptableObject
{
    [Header("GameSettings")]
    public int locksCountMax;
    public int locksCountMin;
    public int keysCountMax;
    public int keysCountMin;
    public int maxLengthMax;
    public int maxLengthMin;
    public int minLengthMax;
    public int minLengthMin;

    [Header("PinSettings")]
    public float wGapMax;
    public float wGapMin;
    public float hGapMax;
    public float hGapMin;
    public float scrollSpeedMax;
    public float scrollSpeedMin;
    public float keysPosMax;
    public float keysPosMin;

    [Header("UISettings")]
    public float uiWidth;
    public float uiHeight;

    public PinLockProperties Add(PinLockController p)
    {
        p.locksCount += Random.Range(locksCountMin, locksCountMax + 1);
        p.keysCount += Random.Range(keysCountMin, keysCountMax + 1);
        p.maxLength += Random.Range(maxLengthMin, maxLengthMax + 1);
        p.minLength += Random.Range(minLengthMin, minLengthMax + 1);
        p.wGap += Random.Range(wGapMin, wGapMax);
        p.hGap += Random.Range(hGapMin, hGapMax);
        p.scrollSpeed += Random.Range(scrollSpeedMin, scrollSpeedMax);
        p.keysPos += Random.Range(keysPosMin, keysPosMax);
        p.uiWidth += uiWidth;
        p.uiHeight += uiHeight;

        return this;
    }
}