using UnityEngine;
using UnityEngine.UI;

public class ChanceGauge : MonoBehaviour
{
    [SerializeField] Slider gauge;
    [SerializeField] float maxChancePoint;
    [SerializeField] float chancePoint;

    private void Update()
    {
        gauge.value = chancePoint / maxChancePoint;
    }

    public bool IsChance()
    {
        return chancePoint >= maxChancePoint;
    }

    public void AddChancePoint(float add)
    {
        chancePoint += add;
        chancePoint = Mathf.Clamp(chancePoint, 0, maxChancePoint);
    }
}
