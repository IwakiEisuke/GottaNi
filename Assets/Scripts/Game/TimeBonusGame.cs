using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBonusGame : GameBase
{
    [Header("GameSettings")]
    [SerializeField] float targetAngle;
    [SerializeField] float angleRange;
    [SerializeField] float speed;

    [Header("AnimationSettings")]
    [SerializeField] float startDuration;
    [SerializeField] float endDuration;

    [Header("Others")]
    [SerializeField] Transform clockHandPivot;
    [SerializeField] Image sector;
    [SerializeField] Material m;
    [SerializeField] SpriteRenderer display;

    private void Start()
    {
        m.SetFloat("_Seed", Random.Range(0f, 100));
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

        Debug.Log(m.GetFloat("_T"));

        clockHandPivot.Rotate(0, 0, -speed * Time.deltaTime);
        SetAngles();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var handAngle = clockHandPivot.rotation.eulerAngles.z; // 0Å`360

            var diff = handAngle - targetAngle;
            diff = Mathf.Abs((180 * 3 + diff) % 360 - 180);

            if (diff < angleRange / 2)
            {
                EndGame();
            }
        }
    }

    public override void EndGame()
    {
        isPlaying = false;
        DOTween.Kill(gameObject);
        DOTween.To(() => m.GetFloat("_T"), x => m.SetFloat("_T", x), 0, endDuration)
            .OnComplete(() => 
            { 
                display.gameObject.SetActive(false); 
                base.EndGame(); 
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
