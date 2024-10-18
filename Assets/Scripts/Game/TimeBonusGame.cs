using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBonusGame : GameBase
{
    [SerializeField] float targetAngle;
    [SerializeField] float angleRange;
    [SerializeField] float speed;
    [SerializeField] Transform clockHandPivot;

    public override void StartGame()
    {
        isPlaying = true;
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        clockHandPivot.Rotate(0, 0, -speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var theta = clockHandPivot.rotation.eulerAngles.z; // 0Å`360

            if (Mathf.Abs(theta - targetAngle) < angleRange / 2)
            {
                EndGame();
            }
        }
    }

    public override void EndGame()
    {
        base.EndGame();
    }
}
