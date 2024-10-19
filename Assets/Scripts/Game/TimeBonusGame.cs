using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBonusGame : GameBase
{
    [SerializeField] float targetAngle;
    [SerializeField] float angleRange;
    [SerializeField] float speed;
    [SerializeField] Transform clockHandPivot;
    [SerializeField] Image sector;

    public override void StartGame()
    {
        isPlaying = true;
    }
    public override void EndGame()
    {
        base.EndGame();
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        clockHandPivot.Rotate(0, 0, -speed * Time.deltaTime);
        SetAngles();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var theta = clockHandPivot.rotation.eulerAngles.z; // 0Å`360

            if (Mathf.Abs(theta - targetAngle) < angleRange / 2)
            {
                EndGame();
            }
        }
    }

    private void SetAngles()
    {
        targetAngle = (360 + targetAngle) % 360;

        if (sector)
        {
            sector.fillAmount = angleRange / 360;
            sector.transform.rotation = Quaternion.Euler(0, 0, 180 + angleRange / 2 + targetAngle);
        }
    }

    private void OnValidate()
    {
        SetAngles();
    }
}
