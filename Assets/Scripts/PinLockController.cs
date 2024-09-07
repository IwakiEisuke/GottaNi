using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 長さがランダムなピンがスクロールし続けるので、それにタイミングを合わせて反対側のピンをがっちりはめると解ける鍵。
/// </summary>
public class PinLockController : MonoBehaviour
{
    [Header("GameSettings")]
    /// <summary>錠側のピンの数</summary>
    public int locksCount;
    /// <summary>鍵側のピンの数</summary>
    public int keysCount;
    /// <summary>最大のピンの長さ</summary>
    public int maxLength;
    /// <summary>最小のピンの長さ</summary>
    public int minLength;

    [Header("PinSettings")]
    public float wGap;
    public float hGap;
    public float scrollSpeed;
    public float keysPos;

    [Header("Object")]
    public GameObject lockPref;
    public GameObject keyPref;
    public Transform keyParent;

    [Header("UISettings")]
    public float uiWidth;
    public float uiHeight;
    public float openDuration;
    public SpriteRenderer frame;
    public float centerX, centerY;

    [Header("GameClear AnimationSettings")]
    public float duration;
    public float strength;
    public int vibrato;

    ///<summary>スクロールの停止フラグ<br></br>ゲームの成功判定とは別</summary>
    [Header("Debug")]
    [SerializeField] bool isScrollPause;

    PinData[] locks, keys;
    Transform mask;
    /// <summary>ゲームの成功判定</summary>
    bool isClear;

    public event UnityAction OnCompleteAction;
    public int maxAddScore;
    public int addScore;

    public bool gameIsCompleteOnMissed;

    public PinLockData pinLockData;

    private void Start()
    {
        transform.position = new Vector3(uiWidth / 2 + centerX, uiHeight / 2 + centerY);

        isScrollPause = true;
        mask = GetComponentInChildren<SpriteMask>().transform;

        // 開始時アニメーション
        DOTween.Sequence(mask) 
            .Append(DOTween.To(() => 0, x => uiWidth = x, uiWidth, openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => 0, x => uiHeight = x, uiHeight, openDuration).SetEase(Ease.Linear))
            .OnComplete(Init);

        uiWidth = 0; // Tween開始までに1フレーム?かかるので、それまでにuiが開かないように0にしておく
        uiHeight = 0; // uiWidthのTween完了まで0にならないので先に0にしておく
    }

    public void Init()
    {
        //錠側のピンを作成する
        var locksLength = new int[locksCount]; // keyの作成に使用する
        locks = new PinData[locksCount];
        for (int i = 0; i < locksCount; i++)
        {
            locksLength[i] = Random.Range(minLength, maxLength + 1);
            locks[i] = PinData.Create(locksLength[i], i, lockPref);
            locks[i].name = "Pin" + i;
        }

        // 鍵側のピンを作成する。pinsLengthから長さを範囲で持ってきてmaxLengthとの差を求める
        var start = Random.Range(0, locksLength.Length);
        keys = new PinData[keysCount];
        for (int i = 0; i < keysCount; i++)
        {
            var keyLength = maxLength - locksLength[(i + start) % locksLength.Length];
            keys[i] = PinData.Create(keyLength + 1, i, keyPref);
            keys[i].transform.SetParent(keyParent);
            keys[i].name = "Pin" + i;
        }

        PinGroup.Create(locks, pinLockData, 1, -1, transform);
        PinGroup.Create(keys, pinLockData, -1, 0, transform);

        isScrollPause = false;
    }

    void Update()
    {
        var uiSize = new Vector2(uiWidth, uiHeight);
        mask.localScale = uiSize;
        mask.localPosition = -uiSize / 2;
        frame.size = uiSize + new Vector2(2, 2);
        frame.transform.localPosition = -uiSize / 2;

        if (!isClear)
        {
            // 振動アニメーションを打ち消してしまうため、成功判定が出てからは処理しない
            transform.position = new Vector3(uiWidth / 2 + centerX, uiHeight / 2 + centerY); // UIの中心を合わせる
        }

        if (!isScrollPause && Input.GetKeyDown(KeyCode.Space)) // 鍵の照合
        {
            isScrollPause = true;

            var offsetY = 0f; // 錠のアニメーション距離
            var offsetX = float.MaxValue; // 鍵のアニメーション距離

            var hitPinCount = 0; // 合致したピンの数
            isClear = true;
            foreach (var key in keys) // 成功・失敗判定
            {
                var keyY = key.transform.position.y;
                var targets = locks.Select(x => Mathf.Abs(x.transform.position.y - keyY)).ToList();
                var index = targets.IndexOf(targets.Min());

                if (key.Length - 1 + locks[index].Length != maxLength) // 失敗
                {
                    isClear = false;
                }
                
                if (offsetY == 0) // 一回処理すればOK
                {
                    offsetY = key.transform.position.y - locks[index].transform.position.y;
                }

                var minLengthAtLocksPin = uiWidth - wGap * (key.Length + locks[index].Length);
                if (minLengthAtLocksPin < offsetX)
                {
                    offsetX = minLengthAtLocksPin;
                    hitPinCount = 1;
                }
                else if (minLengthAtLocksPin == offsetX)
                {
                    hitPinCount++;
                }

                //Debug.Log($"{index} {key.length} {locks[index].length}");
            }

            if (isClear) // scoreを決定
            {
                addScore = maxAddScore * 2;
            }
            else
            {
                addScore = (int)(1f * hitPinCount / keysCount * maxAddScore); // 合致したピンの割合でスコアを決定
            }

            if (gameIsCompleteOnMissed || isClear)
            {
                // 錠の縦移動アニメーション
                foreach (var pin in locks)
                {
                    pin.transform.DOLocalMoveY(offsetY, 0.1f).SetRelative();
                }

                if (isClear) transform.DOShakePosition(duration, strength, vibrato);

                //offsetX = uiWidth - wGap * (maxLength + 1); // 成功時の鍵の横移動の大きさ。ピッタリ嵌まるよう移動距離を設定する
            }
            else
            {
                foreach (var pin in keys) // 失敗時の鍵の横移動の大きさ。半端なところに引っかかるよう移動距離を設定する
                {
                    var offsetKeys = locks.Where(x => Mathf.Abs(x.transform.position.y - pin.transform.position.y) < hGap).ToList();
                    offsetKeys.ForEach(x => offsetX = Mathf.Min(offsetX, uiWidth - pin.Length * wGap - (x.Length * wGap)));
                }
            }

            foreach (var pin in keys) // 鍵の照合アニメーション
            {
                pin.transform.DOLocalMoveX(offsetX, 0.05f).SetRelative().SetEase(Ease.Linear); // 右に動かす。成功時・失敗時共通

                if (!gameIsCompleteOnMissed && !isClear) // 失敗の場合元に戻すアニメーションも再生してポーズを解除
                {
                    DOTween.Sequence(pin)
                   .AppendInterval(0.2f)
                   .Append(pin.transform.DOLocalMoveX(pin.transform.localPosition.x, 0.1f))
                   .OnComplete(() => isScrollPause = false);
                }
            }

            if (gameIsCompleteOnMissed || isClear) // このゲームを終了させる処理
            {
                Invoke(nameof(Complete), duration);
            }
        }
    }

    void Complete() // 終了時アニメーション。アニメーションを終えたらこのゲームオブジェクトを破棄する
    {
        DOTween.Sequence(mask)
            .Append(DOTween.To(() => uiHeight, x => uiHeight = x, 0, openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => uiWidth, x => uiWidth = x, 0, openDuration).SetEase(Ease.Linear))
            .OnComplete(() => {
                OnCompleteAction?.Invoke();
                Destroy(gameObject);
            });
    }
}
