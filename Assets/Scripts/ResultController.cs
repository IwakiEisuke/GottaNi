using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] RectMask2D masking;
    [SerializeField] float x;
    [SerializeField] float y;
    void Start()
    {
        var t = masking.rectTransform;
        t.DOAnchorMax(new Vector2(x, y), 1);
        t.DOAnchorMin(new Vector2(1 - x, 1 - y), 1);
    }
}
