using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinLock : MonoBehaviour
{
    [SerializeField] int count, maxLength, minLength, screenWidth, screenHeight;
    /// <summary>カギが認証するピンの数（カギについているピンの数） </summary>
    [SerializeField] int verifyPinsCount; 
    [SerializeField] GameObject pinPref, mask;
    [SerializeField] float wGap, hGap, scrollSpeed;

    GameObject[] pins;
    int[] pinsLength;

    void Start()
    {
        pinsLength = new int[count];
        pins = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            var thisPinLength = Random.Range(minLength, maxLength + 1);
            var xSize = thisPinLength * wGap;

            var pin = Instantiate(pinPref, transform);
            pin.transform.localScale = new Vector3(xSize, hGap);
            pin.transform.Translate(-xSize / 2f, -(i + 0.5f) * hGap, 0);

            pinsLength[i] = thisPinLength;
            pins[i] = pin;
        }

        var start = Random.Range(0, pinsLength.Length);
        var keyWidth = new int[verifyPinsCount];
        for (int i = 0; i < verifyPinsCount; i++)
        {
            keyWidth[i] = pinsLength[(i + start) % pinsLength.Length];
        }

        Debug.Log(string.Join(' ', keyWidth));
    }

    void Update()
    {
        mask.transform.localScale = new Vector3(screenWidth, screenHeight, 1);

        foreach(var card in pins)
        {
            card.transform.Translate(0, -scrollSpeed * Time.deltaTime, 0);

            if (card.transform.localPosition.y < -screenHeight - hGap / 2)
            {
                card.transform.Translate(0, pins.Length * hGap, 0);
                card.SetActive(false);
            }
            else if (card.transform.localPosition.y < hGap / 2)
            {
                card.SetActive(true);
            }
        }
    }
}
