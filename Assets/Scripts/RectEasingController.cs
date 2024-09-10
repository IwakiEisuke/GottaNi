using DG.Tweening;
using UnityEngine;

public class RectEasingController : MonoBehaviour
{
    [SerializeField] RectTransform[] anchorTargets;
    [SerializeField] RectTransform[] deltaTargets;
    [SerializeField] float duration;
    [SerializeField] Ease ease;
    [SerializeField] MoveType moveType;
    [SerializeField] bool playOnAwake;
    [SerializeField] float timeToPlay;


    void Start()
    {
        if (playOnAwake)
        {
            Tween();
        }
    }

    public void Tween()
    {
        foreach (var t in anchorTargets)
        {
            Anchor(t);
        }

        foreach (var t in deltaTargets)
        {
            Delta(t);
        }
    }

    void Anchor(RectTransform e)
    {
        switch (moveType)
        {
            case MoveType.Open:
                AnchorOpen(e);
                break;
            case MoveType.Close:
                AnchorClose(e);
                break;
        }

        void AnchorOpen(RectTransform t)
        {
            t.DOAnchorMax(Vector2.one, duration).SetEase(ease).SetDelay(timeToPlay);
            t.DOAnchorMin(Vector2.zero, duration).SetEase(ease).SetDelay(timeToPlay);
            var vec = new Vector2(0.5f, 0.5f);
            t.anchorMax = vec;
            t.anchorMin = vec;
        }

        void AnchorClose(RectTransform t)
        {
            var vec = new Vector2(0.5f, 0.5f);
            t.DOAnchorMax(vec, duration).SetEase(ease).SetDelay(timeToPlay);
            t.DOAnchorMin(vec, duration).SetEase(ease).SetDelay(timeToPlay);
        }
    }

    void Delta(RectTransform e)
    {
        switch (moveType)
        {
            case MoveType.Open:
                DeltaOpen(e);
                break;
            case MoveType.Close:
                DeltaClose(e);
                break;
        }

        void DeltaOpen(RectTransform t)
        {
            t.DOSizeDelta(t.sizeDelta, duration).SetEase(ease).SetDelay(timeToPlay);
            t.sizeDelta = Vector2.zero;
        }

        void DeltaClose(RectTransform t)
        {
            t.DOSizeDelta(Vector2.zero, duration).SetEase(ease).SetDelay(timeToPlay);
        }
    }

    enum MoveType
    {
        Open,
        Close
    }
}
