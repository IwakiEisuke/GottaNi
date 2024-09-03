using DG.Tweening;
using System.Linq;
using UnityEngine;

/// <summary>
/// 長さがランダムなピンがスクロールし続けるので、それにタイミングを合わせて反対側のピンをがっちりはめると解ける鍵。
/// </summary>
public class PinLock : MonoBehaviour
{
    [Header("GameSettings")]
    /// <summary>錠側のピンの数</summary>
    [SerializeField] int locksCount;
    /// <summary>鍵側のピンの数</summary>
    [SerializeField] int keysCount;
    /// <summary>最大のピンの長さ</summary>
    [SerializeField] int maxLength;
    /// <summary>最小のピンの長さ</summary>
    [SerializeField] int minLength;

    [Header("PinSettings")]
    [SerializeField] float wGap;
    [SerializeField] float hGap;
    [SerializeField] float scrollSpeed;
    [SerializeField] float keysPos;

    [Header("Object")]
    [SerializeField] GameObject lockPref;
    [SerializeField] GameObject keyPref;
    [SerializeField] Transform keyParent;

    [Header("UISettings")]
    [SerializeField] float uiWidth;
    [SerializeField] float uiHeight;
    [SerializeField] SpriteRenderer frame;

    [Header("VerifyAnimationSettings")]
    [SerializeField] float duration;
    [SerializeField] float strength;
    [SerializeField] int vibrato;

    [Header("Debug")]
    [SerializeField] bool isPause;

    LockPin[] locks, keys;
    Transform mask;

    void Start()
    {
        mask = GetComponentInChildren<SpriteMask>().transform;

        //錠側のピンを作成する
        var locksLength = new int[locksCount]; // keyの作成に使用する
        locks = new LockPin[locksCount];
        for (int i = 0; i < locksCount; i++)
        {
            locksLength[i] = Random.Range(minLength, maxLength + 1);
            locks[i] = CreatePin(locksLength[i], i, 1, lockPref);
            locks[i].name = "Pin" + i;
        }

        // 鍵側のピンを作成する。pinsLengthから長さを範囲で持ってきてmaxLengthとの差を求める
        var start = Random.Range(0, locksLength.Length);
        keys = new LockPin[keysCount];
        for (int i = 0; i < keysCount; i++)
        {
            var keyLength = maxLength - locksLength[(i + start) % locksLength.Length];
            keys[i] = CreatePin(keyLength + 1, i, -1, keyPref);
            keys[i].transform.SetParent(keyParent);
            keys[i].name = "Pin" + i;
            Debug.Log(keyLength);
        }
    }

    void Update()
    {
        var uiSize = new Vector2(uiWidth, uiHeight);
        mask.localScale = uiSize;
        mask.localPosition = -uiSize / 2;
        frame.size = uiSize + new Vector2(2, 2);
        frame.transform.localPosition = -uiSize / 2;

        if (!isPause) // ピンをスクロールさせる
        {
            foreach (var pin in locks)
            {
                var scrollSize = hGap * locksCount;
                pin.transform.localScale = new Vector3(wGap * pin.length, hGap);
                pin.pos += Time.deltaTime * scrollSpeed;
                pin.pos %= locksCount;
                pin.transform.localPosition = new Vector3(GetPinX(pin, 1), Mathf.Lerp(hGap, -scrollSize + hGap, pin.pos / locksCount));
            }

            foreach (var pin in keys)
            {
                var scrollSize = hGap * locksCount;
                pin.transform.localScale = new Vector3(wGap * pin.length, hGap);
                pin.transform.localPosition = new Vector3(GetPinX(pin, -1), Mathf.Lerp(-keysCount / 2f * hGap, -uiHeight + keysCount / 2f * hGap, keysPos) - (pin.pos - keysCount / 2) * hGap);
            }
        }

        if (!isPause && Input.GetKeyDown(KeyCode.Space)) // 鍵の照合
        {
            isPause = true;

            var offsetY = 0f;
            var offsetX = float.MaxValue;

            bool verify = true;
            foreach (var key in keys)
            {
                var keyY = key.transform.position.y;
                var pls = locks.Select(x => Mathf.Abs(x.transform.position.y - keyY)).ToList();
                var index = pls.IndexOf(pls.Min());

                if (key.length - 1 + locks[index].length != maxLength)
                {
                    verify = false;
                }
                else
                {
                    offsetY = key.transform.position.y - locks[index].transform.position.y; // 成功時にしか使用しない
                }

                //Debug.Log($"{index} {key.length} {locks[index].length}");
            }

            if (verify)
            {
                // 錠の縦移動アニメーション
                foreach (var pin in locks)
                {
                    pin.transform.DOLocalMoveY(offsetY, 0.1f).SetRelative();
                }

                transform.DOShakePosition(duration, strength, vibrato);

                offsetX = uiWidth - wGap * (maxLength + 1);
            }
            else
            {
                foreach (var key in keys)
                {
                    var offsetKeys = locks.Where(x => Mathf.Abs(x.transform.position.y - key.transform.position.y) < hGap).ToList();
                    offsetKeys.ForEach(x => offsetX = Mathf.Min(offsetX, uiWidth - key.length * wGap - (x.length * wGap)));
                }
            }

            foreach (var pin in keys) // 鍵の照合アニメーション
            {
                pin.transform.DOLocalMoveX(offsetX, 0.05f).SetRelative().SetEase(Ease.Linear); // 右に動かす

                if (!verify) // 失敗の場合元に戻すアニメーションも再生してポーズを解除
                {
                    DOTween.Sequence(pin)
                   .AppendInterval(0.2f)
                   .Append(pin.transform.DOLocalMoveX(pin.transform.localPosition.x, 0.1f))
                   .OnComplete(() => isPause = false);
                }
            }
        }
    }

    LockPin CreatePin(int Length, float pos, float right, GameObject pinPref)
    {
        var pinSize = Length * wGap;

        var pin = Instantiate(pinPref, transform).GetComponent<LockPin>();
        pin.length = Length;
        pin.pos = pos;
        pin.transform.localScale = new Vector3(pinSize, hGap);

        pin.transform.Translate(0, -pos * hGap, 0); // 縦に並べる
        pin.transform.localPosition = GetSortedPinPos(pin, right, 0);

        return pin;
    }


    float GetPinX(LockPin pin, float right)
    {
        var pinSize = pin.length * wGap;
        var x = (pinSize - uiWidth) / 2f * -right - uiWidth / 2f; // 左右ソート
        return x;
    }

    float GetPinY(LockPin pin, float up)
    {
        var y = -pin.pos * hGap - uiHeight * up;
        return y;
    }

    Vector2 GetSortedPinPos(LockPin pin, float right, float up)
    {
        var x = GetPinX(pin, right);
        var y = GetPinY(pin, up);
        return new Vector2(x, y);
    }
}
