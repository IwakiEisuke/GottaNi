using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChanceGauge : MonoBehaviour
{
    [SerializeField] Slider gauge;
    [SerializeField] float maxChancePoint;
    [SerializeField] float chancePoint;
    [SerializeField] float decreaseSpeed;
    [SerializeField] float tweenDuration;
    public bool IsChance { get; private set; }
    bool isTweening;

    private void Update()
    {
        if (!IsChance)
        {
            chancePoint -= decreaseSpeed * Time.deltaTime;
        }

        if (!isTweening) gauge.value = chancePoint / maxChancePoint;
    }

    public void AddChancePoint(float add)
    {
        if (chancePoint < 0) chancePoint = 0;
        chancePoint += add;
        chancePoint = Mathf.Clamp(chancePoint, 0, maxChancePoint);

        var diff = chancePoint < maxChancePoint ? decreaseSpeed * tweenDuration : 0;
        gauge.DOValue((chancePoint - diff) / maxChancePoint, tweenDuration).OnComplete(() => isTweening = false);
        isTweening = true;

        if (chancePoint >= maxChancePoint)
        {
            AudioManager.Play(SoundType.GaugeFull);
            IsChance = true;
        }
    }

    public void ResetGauge()
    {
        IsChance = false;
        chancePoint = 0;
    }
}
