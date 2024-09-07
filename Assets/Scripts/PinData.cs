using UnityEngine;
using UnityEngine.UIElements;

public class PinData : MonoBehaviour
{
    int length;
    float pos;

    public PinGroup group;
    public PinLockData lockData;

    public int Length => length;
    public float Pos => pos;

    public static PinData Create(int length, float pos, GameObject pinPref)
    {
        var pin = Instantiate(pinPref).GetComponent<PinData>();
        pin.length = length;
        pin.pos = pos;
        return pin;
    }

    public void Scroll(float scrollSpeed, int pinsCount)
    {
        pos += Time.deltaTime * scrollSpeed;
        pos %= pinsCount;
        SetPos();
    }

    void SetPos()
    {
        var scrollSize = lockData.hGap * lockData.locksCount;
        transform.localScale = new Vector3(lockData.wGap * length, lockData.hGap);
        transform.localPosition = GetSortedPos(group.right, group.down);
    }

    float GetX(float right) // ècÇëµÇ¶ÇÈÉ\Å[Ég
    {
        var pinSize = length * lockData.wGap;
        var x = (pinSize - lockData.uiWidth) / 2f * -right - lockData.uiWidth / 2f;
        return x;
    }

    float GetY(float down) // ècÇ…ï¿Ç◊ÇÈ
    {
        var y = Mathf.Lerp(-(group.GetLength() / 2f - 1) * lockData.hGap, -lockData.uiHeight + (group.GetLength() / 2f - 1) * lockData.hGap, (down + 1) / 2) - (Pos - group.GetLength() / 2) * lockData.hGap;
        return y;
    }

    Vector2 GetSortedPos(float right, float down)
    {
        var x = GetX(right);
        var y = GetY(down);
        return new Vector2(x, y);
    }
}
