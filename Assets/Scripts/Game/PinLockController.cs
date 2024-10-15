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
    [SerializeField] PinLockGameProperties p;
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
        transform.position = new Vector3(p.uiWidth / 2 + p.centerX, p.uiHeight / 2 +  p.centerY);

        isScroll = false;
        mask = GetComponentInChildren<SpriteMask>().transform;

        AudioManager.Play(SoundType.OpenGame);

        // 開始時アニメーション
        DOTween.Sequence(mask)
            .Append(DOTween.To(() => 0, x => p.uiWidth = x, p.uiWidth, p.openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => 0, x => p.uiHeight = x, p.uiHeight, p.openDuration).SetEase(Ease.Linear))
            .OnComplete(InitPin);

        p.uiWidth = 0; // Tween開始までに1フレーム?かかるので、それまでにuiが開かないように0にしておく
        p.uiHeight = 0; // uiWidthのTween完了まで0にならないので先に0にしておく
    }

    void Update()
    {
        SetUISize();

        if (!isShaking)
        {
            // 振動アニメーションを打ち消さないように
            transform.position = new Vector3(p.uiWidth / 2 + p.centerX, p.uiHeight / 2 + p.centerY); // UIの中心を合わせる
        }

        if (isScroll)
        {
            // ピンをスクロールさせる
            foreach (var pin in locks)
            {
                pin.AddPos(p.scrollSpeed, p.locksCount);
            }

            PinSetPos(locks, 1, -1);
            PinSetPos(keys, -1, p.keysPos);
        }

        if (isScroll && Input.GetKeyDown(KeyCode.Space)) // 鍵の照合
        {
            isScroll = false;

            Matching(out var offset, out var hitPinCount);

            SetScore(hitPinCount);

            Animation(offset.x, offset.y);

            if (p.gameCloseEvenIfMissing || result.success) // このゲームを終了させる処理
            {
                Invoke(nameof(Complete), p.duration);
            }
        }
    }

    public void InitPin()
    {
        //錠側のピンを作成する
        var locksLength = new int[p.locksCount]; // keyの作成に使用する
        locks = new PinData[p.locksCount];
        for (int i = 0; i < p.locksCount; i++)
        {
            locksLength[i] = Random.Range(p.minLength, p.maxLength + 1);
            locks[i] = PinData.CreatePin(locksLength[i], i, p.lockPref, transform);
            locks[i].name = "Pin" + i;
        }

        // 鍵側のピンを作成する。pinsLengthから長さを範囲で持ってきてmaxLengthとの差を求める
        var start = Random.Range(0, locksLength.Length);
        keys = new PinData[p.keysCount];
        for (int i = 0; i < p.keysCount; i++)
        {
            var keyLength = p.maxLength - locksLength[(i + start) % locksLength.Length];
            keys[i] = PinData.CreatePin(keyLength + 1, i, p.keyPref, p.keyParent);
            keys[i].name = "Pin" + i;
        }

        isScroll = true;
    }

    void SetUISize()
    {
        var uiSize = new Vector2(p.uiWidth, p.uiHeight);
        mask.localScale = uiSize;
        mask.localPosition = -uiSize / 2;
        p.frame.size = uiSize + new Vector2(2, 2);
        p.frame.transform.localPosition = -uiSize / 2;
        p.backGround.size = uiSize;
        p.backGround.material.SetVector("_ScrollSpeed", new Vector2(p.backGroundSpeedX, p.backGroundSpeedY));
        p.backGround.transform.localPosition = -uiSize / 2;
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

            if (key.length - 1 + locks[index].length != p.maxLength) // 失敗
            {
                result.success = false;
            }

            if (offset.y == 0) // 一回処理すればOK
            {
                offset.y = key.transform.position.y - locks[index].transform.position.y;
            }

            var minLengthAtLocksPin = p.uiWidth - p.wGap * (key.length + locks[index].length);
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
            result.score = (int)(p.maxAddScore * p.multiplier);
        }
        else
        {
            result.score = (int)(1f * hitPinCount / p.keysCount * p.maxAddScore); // 合致したピンの割合でスコアを決定
        }

        result.chancePoint = 1f * hitPinCount / p.keysCount * p.maxAddChancePoint;
    }

    void Animation(float offsetX, float offsetY)
    {
        var moveDuration = 0.1f;
        // アニメーション
        if (p.gameCloseEvenIfMissing || result.success)
        {

            // 錠の縦移動アニメーション
            foreach (var pin in locks)
            {
                pin.transform.DOLocalMoveY(offsetY, moveDuration).SetRelative();
            }

            if (result.success)
            {
                isShaking = true;
                transform.DOShakePosition(p.duration, p.strength, p.vibrato).OnComplete(() => isShaking = false);
            }
        }
        else
        {
            foreach (var pin in keys) // 失敗時の鍵の横移動の大きさ。半端なところに引っかかるよう移動距離を設定する
            {
                var offsetKeys = locks.Where(x => Mathf.Abs(x.transform.position.y - pin.transform.position.y) < p.hGap).ToList();
                offsetKeys.ForEach(x => offsetX = Mathf.Min(offsetX, p.uiWidth - pin.length * p.wGap - (x.length * p.wGap)));
            }
        }

        foreach (var pin in keys) // 鍵の照合アニメーション
        {
            pin.transform.DOLocalMoveX(offsetX, moveDuration / 2f).SetRelative().SetEase(Ease.Linear); // 右に動かす。成功時・失敗時共通

            if (!p.gameCloseEvenIfMissing && !result.success) // 失敗の場合元に戻すアニメーションも再生してポーズを解除
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
            .Append(DOTween.To(() => p.uiHeight, x => p.uiHeight = x, 0, p.openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => p.uiWidth, x => p.uiWidth = x, 0, p.openDuration).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                ChangeState(result);
            });
    }

    void PinSetPos(PinData[] pins, float right, float down)
    {
        foreach (var pin in pins)
        {
            pin.GetComponent<SpriteRenderer>().size = new Vector3(p.wGap * pin.length, p.hGap);
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
        var pinSize = pin.length * p.wGap;
        var x = (pinSize - p.uiWidth) / 2f * -right - p.uiWidth / 2f;
        return x;
    }

    float GetPinY(PinData pin, float down) // 縦に並べる
    {
        var y = Mathf.Lerp(-(p.keysCount / 2f - 1) * p.hGap, -p.uiHeight + (p.keysCount / 2f - 1) * p.hGap, (down + 1) / 2) - (pin.pos - p.keysCount / 2) * p.hGap;
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
