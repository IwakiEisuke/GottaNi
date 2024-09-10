using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] RectTransform[] anchorTargets;
    [SerializeField] RectTransform[] deltaTargets;
    [SerializeField] float duration;
    [SerializeField] Ease ease;
    void Start()
    {
        foreach (var t in deltaTargets)
        {
            TweenDelta(t);
        }

        foreach (var t in anchorTargets)
        {
            TweenAnchor(t);
        }
    }

    void TweenDelta(RectTransform t)
    {
        t.DOSizeDelta(t.sizeDelta, duration).SetEase(ease);
        t.sizeDelta = Vector2.zero;
    }

    void TweenAnchor(RectTransform t)
    {
        t.DOAnchorMax(Vector2.one, duration).SetEase(ease);
        t.DOAnchorMin(Vector2.zero, duration).SetEase(ease);
        var vec = new Vector2(0.5f, 0.5f);
        t.anchorMax = vec;
        t.anchorMin = vec;
    }
}
