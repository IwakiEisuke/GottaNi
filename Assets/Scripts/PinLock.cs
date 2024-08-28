using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// �����������_���ȃs�����X�N���[����������̂ŁA����Ƀ^�C�~���O�����킹�Ĕ��Α��̃s������������͂߂�Ɖ����錮�B
/// </summary>
public class PinLock : MonoBehaviour
{
    [SerializeField] int count, maxLength, minLength, screenWidth, screenHeight;
    /// <summary>�J�M���F�؂���s���̐��i�J�M�ɂ��Ă���s���̐��j </summary>
    [SerializeField] int verifyPinsCount; 
    [SerializeField] GameObject pinPref, mask;
    [SerializeField] float wGap, hGap, scrollSpeed;

    GameObject[] pins;
    int[] pinsLength;

    void Start()
    {
        void CreatePin(int Length, int i, float right, out GameObject pin)
        {
            var pinSize = Length * wGap;

            pin = Instantiate(pinPref, transform);
            pin.transform.localScale = new Vector3(pinSize, hGap);

            pin.transform.Translate(0, -(i + 0.5f) * hGap, 0); // �c�ɕ��ׂ�
            pin.transform.Translate((pinSize - screenWidth) / 2f * right - screenWidth / 2f, 0, 0); // ���E�\�[�g
            //pin.transform.Translate(0, 0, 0); //�㉺�\�[�g
        }

        //�����̃s�����쐬����
        pinsLength = new int[count];
        pins = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            pinsLength[i] = Random.Range(minLength, maxLength + 1);
            CreatePin(pinsLength[i], i, -1, out pins[i]);
        }

        // �����̃s�����쐬����BpinsLength���璷����͈͂Ŏ����Ă���maxLength�Ƃ̍������߂�
        var start = Random.Range(0, pinsLength.Length);
        var keysLength = new int[verifyPinsCount];
        var keys = new GameObject[verifyPinsCount];
        for (int i = 0; i < verifyPinsCount; i++)
        {
            keysLength[i] = maxLength - pinsLength[(i + start) % pinsLength.Length];
            CreatePin(keysLength[i] + 1, i, 1, out keys[i]);
        }

        Debug.Log(string.Join(' ', keysLength));
    }

    void Update()
    {
        mask.transform.localScale = new Vector3(screenWidth, screenHeight, 1);

        // �s�����X�N���[��������
        foreach(var pin in pins)
        {
            pin.transform.Translate(0, -scrollSpeed * Time.deltaTime, 0);

            if (pin.transform.localPosition.y < -screenHeight - hGap / 2) // �s�����͈͊O�ɏo�����ɖ߂�
            {
                pin.transform.Translate(0, pins.Length * hGap, 0);
                pin.SetActive(false); // ��\���ɂ���
            }
            else if (pin.transform.localPosition.y < hGap / 2) // �s�����͈͓��ɓ�������
            {
                pin.SetActive(true); // �\������
            }
        }
    }
}
