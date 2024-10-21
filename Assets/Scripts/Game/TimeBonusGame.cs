using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBonusGame : GameBase
{
    [Header("GameSettings")]
    [SerializeField] float targetAngle;
    [SerializeField] float minAngleRange = 20;
    [SerializeField] float maxAngleRange = 45;
    float angleRange;
    [SerializeField] float minSpeed = 360;
    [SerializeField] float maxSpeed = 360 * 2;
    float speed;

    [Header("AnimationSettings")]
    [SerializeField] float startDuration;
    [SerializeField] float endDuration;

    [Header("Others")]
    [SerializeField] Transform clockHandPivot;
    [SerializeField] Image sector;
    [SerializeField] Material m;
    [SerializeField] SpriteRenderer display;

    new bool isPlaying = false; // これが無いとなんか開幕"true"になる。GameBaseの方で"false"に初期化してるのだけど。（PinLockControllerの方では"false"になってるっぽい?）

    private void Start()
    {
        m.SetFloat("_Seed", Random.Range(0f, 100));

        speed = Random.Range(minSpeed, maxSpeed);
        targetAngle = Random.Range(0f, 360f);
        angleRange = Random.Range(minAngleRange, maxAngleRange);
        SetAngles();

        DOTween.To(() => 0f, x => m.SetFloat("_T", x), 2, startDuration);
    }

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
        SetAngles();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var handAngle = clockHandPivot.rotation.eulerAngles.z; // 0～360

            var diff = handAngle - targetAngle;
            diff = Mathf.Abs((180 * 3 + diff) % 360 - 180);

            if (diff < angleRange / 2)
            {
                result.time = 10;
                result.success = true;
            }
            else
            {
                result.time = 0;
                result.success = false;
            }

            CompleteGame();
        }
    }

    public override void CompleteGame()
    {
        isPlaying = false;
        base.CompleteGame();
    }

    public override void PlayClosingAnimation()
    {
        DOTween.Kill(gameObject);
        DOTween.To(() => m.GetFloat("_T"), x => m.SetFloat("_T", x), 0, endDuration)
            .OnComplete(() =>
            {
                if (display) display.gameObject.SetActive(false);
                base.PlayClosingAnimation();
            });
    }

    private void SetAngles()
    {
        targetAngle = (180 * 3 + targetAngle) % 360 - 180;

        if (sector)
        {
            sector.fillAmount = angleRange / 360;
            sector.transform.rotation = Quaternion.Euler(0, 0, 180 + angleRange / 2 + targetAngle);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetAngles();
    }
#endif

}
