using System.Linq;
using UnityEngine;

/// <summary>
/// �����������_���ȃs�����X�N���[����������̂ŁA����Ƀ^�C�~���O�����킹�Ĕ��Α��̃s������������͂߂�Ɖ����錮�B
/// </summary>
public class PinLock : MonoBehaviour
{
    [SerializeField] int pinCount, maxLength, minLength;
    [SerializeField] float uiWidth, uiHeight;
    /// <summary>�J�M���F�؂���s���̐��i�J�M�ɂ��Ă���s���̐��j </summary>
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

            pin.transform.Translate(0, -(i + 0.5f) * hGap, 0); // �c�ɕ��ׂ�
            pin.transform.Translate((pinSize - uiWidth) / 2f * right - uiWidth / 2f, 0, 0); // ���E�\�[�g
            //pin.transform.Translate(0, 0, 0); //�㉺�\�[�g
        }

        //�����̃s�����쐬����
        var pinsLength = new int[pinCount]; // key�̍쐬�Ɏg�p����
        pins = new LockPin[pinCount];
        for (int i = 0; i < pinCount; i++)
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

        // �s�����X�N���[��������
        foreach (var pin in pins)
        {
            pin.transform.Translate(0, -scrollSpeed * Time.deltaTime, 0);

            var scrollSize = hGap * pinCount;
            if (pin.transform.localPosition.y < -scrollSize + hGap / 2) // �s�����͈͊O�ɏo�����ɖ߂�
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
