using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBonusGame : MonoBehaviour
{
    [SerializeField] float targetAngle;
    [SerializeField] float angleRange;
    [SerializeField] float speed;
    [SerializeField] Transform clockHandPivot;

    void Start()
    {

    }

    void Update()
    {
        clockHandPivot.Rotate(0, 0, -speed * Time.deltaTime);
    }
}
