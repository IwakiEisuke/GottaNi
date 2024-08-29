using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
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

            pin.transform.Translate(0, -(i + 0.5f) * hGap, 0); // �c�ɕ��ׂ�
            pin.transform.Translate((pinSize - screenWidth) / 2f * right - screenWidth / 2f, 0, 0); // ���E�\�[�g
            //pin.transform.Translate(0, 0, 0); //�㉺�\�[�g
        }

        //�����̃s�����쐬����
        var pinsLength = new int[count]; // key�̍쐬�Ɏg�p����
        pins = new LockPin[count];
        for (int i = 0; i < count; i++)
        {
            pinsLength[i] = Random.Range(minLength, maxLength + 1);
            CreatePin(pinsLength[i], i, -1, out pins[i]);
        }

        // �����̃s�����쐬����BpinsLength���璷����͈͂Ŏ����Ă���maxLength�Ƃ̍������߂�
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

        // �s�����X�N���[��������
        foreach(var pin in pins)
        {
            pin.transform.Translate(0, -scrollSpeed * Time.deltaTime, 0);

            if (pin.transform.localPosition.y < -screenHeight - hGap / 2) // �s�����͈͊O�ɏo�����ɖ߂�
            {
                pin.transform.Translate(0, pins.Length * hGap, 0);
                pin.gameObject.SetActive(false); // ��\���ɂ���
            }
            else if (pin.transform.localPosition.y < hGap / 2) // �s�����͈͓��ɓ�������
            {
                pin.gameObject.SetActive(true); // �\������
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
