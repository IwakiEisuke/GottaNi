using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPin : MonoBehaviour
{
    int length;

    public void Set(int length)
    {
        this.length = length;
    }

    public int GetLength()
    {
        return length;
    }
}
