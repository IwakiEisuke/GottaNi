using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PinGroup : MonoBehaviour
{
    PinData[] pins;
    PinLockData lockData;

    public float right;
    public float down;

    public static PinGroup Create(PinData[] pins, PinLockData lockData, float right, float down, Transform parent)
    {
        var instance = new GameObject("group").AddComponent<PinGroup>();
        instance.pins = pins;
        instance.lockData = lockData;

        foreach (var p in pins)
        {
            p.group = instance;
            p.lockData = lockData;
            p.transform.parent = instance.transform;
        }

        instance.transform.SetParent(parent);
        instance.transform.localPosition = Vector3.zero;

        instance.right = right;
        instance.down = down;

        return instance;
    }

    public static void Create(PinLockData lockData)
    {
        var instance = new GameObject("group").AddComponent<PinGroup>();
        instance.lockData = lockData;
    }

    private void Update()
    {
        if (!lockData.isScrollPause)
        {
            ScrollPins();
        }
    }

    void ScrollPins()
    {
        foreach (var pin in pins)
        {
            pin.Scroll(lockData.scrollSpeed, pins.Length);
        }
    }

    public int GetLength()
    {
        return pins.Length;
    }
}
