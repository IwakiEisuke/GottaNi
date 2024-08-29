using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 長さがランダムなピンがスクロールし続けるので、それにタイミングを合わせて反対側のピンをがっちりはめると解ける鍵。
/// </summary>
public class PinLock : MonoBehaviour
{
    [SerializeField] int count, maxLength, minLength, screenWidth, screenHeight;
    /// <summary>カギが認証するピンの数（カギについているピンの数） </summary>
    [SerializeField] int verifyPinsCount; 
    [SerializeField] GameObject pinPref, mask;
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
            pin.transform.Translate((pinSize - screenWidth) / 2f * right - screenWidth / 2f, 0, 0); // 左右ソート
            //pin.transform.Translate(0, 0, 0); //上下ソート
        }

        //錠側のピンを作成する
        var pinsLength = new int[count]; // keyの作成に使用する
        pins = new LockPin[count];
        for (int i = 0; i < count; i++)
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
            Debug.Log(keyLength);
        }
    }

    void Update()
    {
        mask.transform.localScale = new Vector3(screenWidth, screenHeight, 1);

        // ピンをスクロールさせる
        foreach(var pin in pins)
        {
            pin.transform.Translate(0, -scrollSpeed * Time.deltaTime, 0);

            if (pin.transform.localPosition.y < -screenHeight - hGap / 2) // ピンが範囲外に出たら上に戻す
            {
                pin.transform.Translate(0, pins.Length * hGap, 0);
                pin.gameObject.SetActive(false); // 非表示にする
            }
            else if (pin.transform.localPosition.y < hGap / 2) // ピンが範囲内に入ったら
            {
                pin.gameObject.SetActive(true); // 表示する
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
