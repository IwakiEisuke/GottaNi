using System.Linq;
using UnityEngine;

/// <summary>
/// 長さがランダムなピンがスクロールし続けるので、それにタイミングを合わせて反対側のピンをがっちりはめると解ける鍵。
/// </summary>
public class PinLock : MonoBehaviour
{
    [SerializeField] int pinCount, maxLength, minLength;
    [SerializeField] float uiWidth, uiHeight;
    /// <summary>カギが認証するピンの数（カギについているピンの数） </summary>
    [SerializeField] int verifyPinsCount;
    [SerializeField] GameObject pinPref, mask;
    [SerializeField] SpriteRenderer frame;
    [SerializeField] Transform keyParent;
    [SerializeField] float wGap, hGap, scrollSpeed;

    LockPin[] pins, keys;
    //int[] pinsLength, keysLength;


    void Start()
    {
        void CreatePin(int Length, int i, float right, out LockPin pin)
        {
            var pinSize = Length * wGap;

            pin = Instantiate(pinPref, transform).GetComponent<LockPin>();
            pin.Set(Length);
            pin.name = "LockPin" + i;
            pin.transform.localScale = new Vector3(pinSize, hGap);

            pin.transform.Translate(0, -(i + 0.5f) * hGap, 0); // 縦に並べる
            pin.transform.Translate((pinSize - uiWidth) / 2f * right - uiWidth / 2f, 0, 0); // 左右ソート
            //pin.transform.Translate(0, 0, 0); //上下ソート
        }

        //錠側のピンを作成する
        var pinsLength = new int[pinCount]; // keyの作成に使用する
        pins = new LockPin[pinCount];
        for (int i = 0; i < pinCount; i++)
        {
            pinsLength[i] = Random.Range(minLength, maxLength + 1);
            CreatePin(pinsLength[i], i, -1, out pins[i]);
        }

        // 鍵側のピンを作成する。pinsLengthから長さを範囲で持ってきてmaxLengthとの差を求める
        var start = Random.Range(0, pinsLength.Length);
        keys = new LockPin[verifyPinsCount];
        for (int i = 0; i < verifyPinsCount; i++)
        {
            var keyLength = maxLength - pinsLength[(i + start) % pinsLength.Length];
            CreatePin(keyLength + 1, i, 1, out keys[i]);
            keys[i].transform.SetParent(keyParent);
            Debug.Log(keyLength);
        }
    }

    void Update()
    {
        mask.transform.localScale = new Vector2(uiWidth, uiHeight);
        mask.transform.localPosition = new Vector2(-uiWidth / 2, -uiHeight / 2);
        frame.size = new Vector2(uiWidth + 2, uiHeight + 2);
        frame.transform.localPosition = new Vector2(-uiWidth / 2, -uiHeight / 2);

        // ピンをスクロールさせる
        foreach (var pin in pins)
        {
            pin.transform.Translate(0, -scrollSpeed * Time.deltaTime, 0);

            var scrollSize = hGap * pinCount;
            if (pin.transform.localPosition.y < -scrollSize + hGap / 2) // ピンが範囲外に出たら上に戻す
            {
                pin.transform.Translate(0, scrollSize, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool verify = true;
            foreach (var key in keys)
            {
                var keyY = key.transform.position.y;
                var pls = pins.Select(x => Mathf.Abs(x.transform.position.y - keyY)).ToList();
                var index = pls.IndexOf(pls.Min());
                Debug.Log($"{index} {key.GetLength()} {pins[index].GetLength()}");
                if (key.GetLength() - 1 + pins[index].GetLength() != maxLength)
                {
                    verify = false;
                    break;
                }
            }

            if (verify)
            {

            }

            Debug.Log(verify);
        }
    }
}
