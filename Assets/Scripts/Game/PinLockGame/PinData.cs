using Unity.VisualScripting;
using UnityEngine;

public class PinData : MonoBehaviour
{
    public int length { get; private set; }
    public float pos { get; private set; }

    public static PinData CreatePin(int Length, float pos, GameObject pinPref, Transform parent = default)
    {
        var pin = Instantiate(pinPref, parent).GetComponent<PinData>();
        pin.length = Length;
        pin.pos = pos;
        return pin;
    }

    public void AddPos(float add, int cycle)
    {
        pos += Time.deltaTime * add;
        pos %= cycle;
    }
}
