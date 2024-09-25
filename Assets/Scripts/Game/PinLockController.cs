using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 長さがランダムなピンがスクロールし続けるので、それにタイミングを合わせて反対側のピンをがっちりはめると解ける鍵。
/// </summary>
public class PinLockController : ResultSender
{
    [SerializeField] PinLockGameProperties aaa;
    [SerializeField] UnityEvent OnCompleteEvent;
    ///<summary>スクロールの停止フラグ<br></br>ゲームの成功判定とは別</summary>
    [Header("Debug")]
    [SerializeField] bool isScroll;

    GameSectionResult result;
    PinData[] locks, keys;
    bool isShaking;

    Transform mask;

    private void Start()
    {
        transform.position = new Vector3(aaa.uiWidth / 2 + aaa.centerX, aaa.uiHeight / 2 +  aaa.centerY);

        isScroll = false;
        mask = GetComponentInChildren<SpriteMask>().transform;

        AudioManager.Play(SoundType.OpenGame);

        // 開始時アニメーション
        DOTween.Sequence(mask)
            .Append(DOTween.To(() => 0, x => aaa.uiWidth = x, aaa.uiWidth, aaa.openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => 0, x => aaa.uiHeight = x, aaa.uiHeight, aaa.openDuration).SetEase(Ease.Linear))
            .OnComplete(InitPin);

        aaa.uiWidth = 0; // Tween開始までに1フレーム?かかるので、それまでにuiが開かないように0にしておく
        aaa.uiHeight = 0; // uiWidthのTween完了まで0にならないので先に0にしておく
    }

    void Update()
    {
        SetUISize();

        if (!isShaking)
        {
            // 振動アニメーションを打ち消さないように
            transform.position = new Vector3(aaa.uiWidth / 2 + aaa.centerX, aaa.uiHeight / 2 + aaa.centerY); // UIの中心を合わせる
        }

        if (isScroll)
        {
            // ピンをスクロールさせる
            foreach (var pin in locks)
            {
                pin.AddPos(aaa.scrollSpeed, aaa.locksCount);
            }

            PinSetPos(locks, 1, -1);
            PinSetPos(keys, -1, aaa.keysPos);
        }

        if (isScroll && Input.GetKeyDown(KeyCode.Space)) // 鍵の照合
        {
            isScroll = false;

            Matching(out var offset, out var hitPinCount);

            SetScore(hitPinCount);

            Animation(offset.x, offset.y);

            if (aaa.gameCloseEvenIfMissing || result.success) // このゲームを終了させる処理
            {
                Invoke(nameof(Complete), aaa.duration);
            }
        }
    }

    public void InitPin()
    {
        //錠側のピンを作成する
        var locksLength = new int[aaa.locksCount]; // keyの作成に使用する
        locks = new PinData[aaa.locksCount];
        for (int i = 0; i < aaa.locksCount; i++)
        {
            locksLength[i] = Random.Range(aaa.minLength, aaa.maxLength + 1);
            locks[i] = PinData.CreatePin(locksLength[i], i, aaa.lockPref, transform);
            locks[i].name = "Pin" + i;
        }

        // 鍵側のピンを作成する。pinsLengthから長さを範囲で持ってきてmaxLengthとの差を求める
        var start = Random.Range(0, locksLength.Length);
        keys = new PinData[aaa.keysCount];
        for (int i = 0; i < aaa.keysCount; i++)
        {
            var keyLength = aaa.maxLength - locksLength[(i + start) % locksLength.Length];
            keys[i] = PinData.CreatePin(keyLength + 1, i, aaa.keyPref, aaa.keyParent);
            keys[i].name = "Pin" + i;
        }

        isScroll = true;
    }

    void SetUISize()
    {
        var uiSize = new Vector2(aaa.uiWidth, aaa.uiHeight);
        mask.localScale = uiSize;
        mask.localPosition = -uiSize / 2;
        aaa.frame.size = uiSize + new Vector2(2, 2);
        aaa.frame.transform.localPosition = -uiSize / 2;
        aaa.backGround.size = uiSize;
        aaa.backGround.material.SetVector("_ScrollSpeed", new Vector2(aaa.backGroundSpeedX, aaa.backGroundSpeedY));
        aaa.backGround.transform.localPosition = -uiSize / 2;
    }

    /// <summary>
    /// 成功・失敗判定
    /// </summary>
    /// <param name="offset">x:錠のアニメーション距離<br></br>y:鍵のアニメーション距離</param>
    /// <param name="hitPinCount">合致したピンの数</param>
    void Matching(out Vector2 offset, out int hitPinCount)
    {
        offset.x = float.MaxValue; // 鍵のアニメーション距離
        offset.y = 0;

        hitPinCount = 0;
        result.success = true;
        foreach (var key in keys)
        {
            var keyY = key.transform.position.y;
            var targets = locks.Select(x => Mathf.Abs(x.transform.position.y - keyY)).ToList();
            var index = targets.IndexOf(targets.Min());

            if (key.length - 1 + locks[index].length != aaa.maxLength) // 失敗
            {
                result.success = false;
            }

            if (offset.y == 0) // 一回処理すればOK
            {
                offset.y = key.transform.position.y - locks[index].transform.position.y;
            }

            var minLengthAtLocksPin = aaa.uiWidth - aaa.wGap * (key.length + locks[index].length);
            if (minLengthAtLocksPin < offset.x)
            {
                offset.x = minLengthAtLocksPin;
                hitPinCount = 1;
            }
            else if (minLengthAtLocksPin == offset.x)
            {
                hitPinCount++;
            }
        }
    }

    void SetScore(int hitPinCount)
    {
        // scoreを決定
        if (result.success)
        {
            result.score = (int)(aaa.maxAddScore * aaa.multiplier);
        }
        else
        {
            result.score = (int)(1f * hitPinCount / aaa.keysCount * aaa.maxAddScore); // 合致したピンの割合でスコアを決定
        }

        result.chancePoint = 1f * hitPinCount / aaa.keysCount * aaa.maxAddChancePoint;
    }

    void Animation(float offsetX, float offsetY)
    {
        var moveDuration = 0.1f;
        // アニメーション
        if (aaa.gameCloseEvenIfMissing || result.success)
        {

            // 錠の縦移動アニメーション
            foreach (var pin in locks)
            {
                pin.transform.DOLocalMoveY(offsetY, moveDuration).SetRelative();
            }

            if (result.success)
            {
                isShaking = true;
                transform.DOShakePosition(aaa.duration, aaa.strength, aaa.vibrato).OnComplete(() => isShaking = false);
            }
        }
        else
        {
            foreach (var pin in keys) // 失敗時の鍵の横移動の大きさ。半端なところに引っかかるよう移動距離を設定する
            {
                var offsetKeys = locks.Where(x => Mathf.Abs(x.transform.position.y - pin.transform.position.y) < aaa.hGap).ToList();
                offsetKeys.ForEach(x => offsetX = Mathf.Min(offsetX, aaa.uiWidth - pin.length * aaa.wGap - (x.length * aaa.wGap)));
            }
        }

        foreach (var pin in keys) // 鍵の照合アニメーション
        {
            pin.transform.DOLocalMoveX(offsetX, moveDuration / 2f).SetRelative().SetEase(Ease.Linear); // 右に動かす。成功時・失敗時共通

            if (!aaa.gameCloseEvenIfMissing && !result.success) // 失敗の場合元に戻すアニメーションも再生してポーズを解除
            {
                DOTween.Sequence(pin)
               .AppendInterval(0.2f)
               .Append(pin.transform.DOLocalMoveX(pin.transform.localPosition.x, 0.1f))
               .OnComplete(() => isScroll = true);
            }
        }

        AudioManager.Play(result.success ? SoundType.MatchingSuccess : SoundType.MatchingFailure);
    }

    void Complete() // 終了時アニメーションと加点等処理
    {
        AudioManager.Play(SoundType.CloseGame);
        OnCompleteEvent.Invoke();

        DOTween.Sequence(mask)
            .Append(DOTween.To(() => aaa.uiHeight, x => aaa.uiHeight = x, 0, aaa.openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => aaa.uiWidth, x => aaa.uiWidth = x, 0, aaa.openDuration).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                ChangeState(result);
            });
    }

    void PinSetPos(PinData[] pins, float right, float down)
    {
        foreach (var pin in pins)
        {
            pin.GetComponent<SpriteRenderer>().size = new Vector3(aaa.wGap * pin.length, aaa.hGap);
            pin.transform.localPosition = GetSortedPinPos(pin, right, down);
        }
    }
    Vector2 GetSortedPinPos(PinData pin, float right, float down)
    {
        var x = GetPinX(pin, right);
        var y = GetPinY(pin, down);
        return new Vector2(x, y);
    }

    float GetPinX(PinData pin, float right) // 縦を揃えるソート
    {
        var pinSize = pin.length * aaa.wGap;
        var x = (pinSize - aaa.uiWidth) / 2f * -right - aaa.uiWidth / 2f;
        return x;
    }

    float GetPinY(PinData pin, float down) // 縦に並べる
    {
        var y = Mathf.Lerp(-(aaa.keysCount / 2f - 1) * aaa.hGap, -aaa.uiHeight + (aaa.keysCount / 2f - 1) * aaa.hGap, (down + 1) / 2) - (pin.pos - aaa.keysCount / 2) * aaa.hGap;
        return y;
    }

    public void ExitGame()
    {
        DOTween.Kill(gameObject);
        Complete();
    }

    public override void ChangeState(GameSectionResult result)
    {
        NotifyObservers(result);
        gameObject.SetActive(false);
    }
}
