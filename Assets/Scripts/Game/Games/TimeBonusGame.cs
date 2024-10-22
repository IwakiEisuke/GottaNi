using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBonusGame : GameBase
{
    [Header("GameSettings")]
    [SerializeField] int addTimeSeconds = 10;
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
    [SerializeField] float successDuration;
    [SerializeField] float failureDuration;

    [Header("Others")]
    [SerializeField] Transform clockHandPivot;
    [SerializeField] Image sector;
    [SerializeField] Material m;
    [SerializeField] SpriteRenderer display;

    private void Start()
    {
        isPlaying = false;
        m.SetFloat("_Seed", Random.Range(0f, 100));

        speed = Random.Range(minSpeed, maxSpeed);
        targetAngle = Random.Range(0f, 360f);
        angleRange = Random.Range(minAngleRange, maxAngleRange);
        SetAngles();

        DOTween.To(() => 0f, x => m.SetFloat("_T", x), 1.5f, startDuration);
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
            var handAngle = clockHandPivot.rotation.eulerAngles.z; // 0Å`360

            var diff = handAngle - targetAngle;
            diff = Mathf.Abs((180 * 3 + diff) % 360 - 180);

            if (diff < angleRange / 2)
            {
                result.time = addTimeSeconds;
                result.success = true;
                PlaySuccessAnimation();
            }
            else
            {
                result.time = 0;
                result.success = false;
                PlayFailureAnimation();
            }

            Debug.Log(result.success ? "success" : "failure");
        }
    }

    public override void CompleteGame()
    {
        base.CompleteGame();
    }

    private void PlaySuccessAnimation()
    {
        isPlaying = false;
        Invoke(nameof(CompleteGame), successDuration);
    }

    private void PlayFailureAnimation()
    {
        isPlaying = false;
        Invoke(nameof(CompleteGame), failureDuration);
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
