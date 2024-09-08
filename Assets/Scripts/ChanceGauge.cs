using UnityEngine;
using UnityEngine.UI;

public class ChanceGauge : MonoBehaviour
{
    [SerializeField] Slider gauge;
    [SerializeField] float maxChancePoint;
    [SerializeField] float chancePoint;
    [SerializeField] float decreaseSpeed;
    public bool IsChance { get; private set; }

    private void Update()
    {
        if (!IsChance)
        {
            chancePoint -= decreaseSpeed * Time.deltaTime;
        }

        gauge.value = chancePoint / maxChancePoint;
    }

    public void AddChancePoint(float add)
    {
        chancePoint += add;
        chancePoint = Mathf.Clamp(chancePoint, 0, maxChancePoint);

        if (chancePoint >= maxChancePoint)
        {
            IsChance = true;
        }
    }

    public void ResetChance()
    {
        IsChance = false;
        chancePoint = 0;
    }
}
