using DG.Tweening;
using System.Linq;
using UnityEngine;

/// <summary>
/// �����������_���ȃs�����X�N���[����������̂ŁA����Ƀ^�C�~���O�����킹�Ĕ��Α��̃s������������͂߂�Ɖ����錮�B
/// </summary>
public class PinLock : MonoBehaviour
{
    [Header("GameSettings")]
    /// <summary>�����̃s���̐�</summary>
    [SerializeField] int locksCount;
    /// <summary>�����̃s���̐�</summary>
    [SerializeField] int keysCount;
    /// <summary>�ő�̃s���̒���</summary>
    [SerializeField] int maxLength;
    /// <summary>�ŏ��̃s���̒���</summary>
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
    [SerializeField] float openDuration;
    [SerializeField] SpriteRenderer frame;

    [Header("VerifyAnimationSettings")]
    [SerializeField] float duration;
    [SerializeField] float strength;
    [SerializeField] int vibrato;

    [Header("Debug")]
    [SerializeField] bool isPause;

    LockPin[] locks, keys;
    Transform mask;

    private void Awake()
    {
        isPause = true;
        mask = GetComponentInChildren<SpriteMask>().transform;

        // �J�n���A�j���[�V����
        DOTween.Sequence(mask) 
            .Append(DOTween.To(() => uiWidth, x => uiWidth = x, uiWidth, openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => uiHeight, x => uiHeight = x, uiHeight, openDuration).SetEase(Ease.Linear))
            .OnComplete(Init);

        uiWidth = 0;
        uiHeight = 0;
    }

    public void Init()
    {
        //�����̃s�����쐬����
        var locksLength = new int[locksCount]; // key�̍쐬�Ɏg�p����
        locks = new LockPin[locksCount];
        for (int i = 0; i < locksCount; i++)
        {
            locksLength[i] = Random.Range(minLength, maxLength + 1);
            locks[i] = CreatePin(locksLength[i], i, 1, -1, lockPref);
            locks[i].name = "Pin" + i;
        }

        // �����̃s�����쐬����BpinsLength���璷����͈͂Ŏ����Ă���maxLength�Ƃ̍������߂�
        var start = Random.Range(0, locksLength.Length);
        keys = new LockPin[keysCount];
        for (int i = 0; i < keysCount; i++)
        {
            var keyLength = maxLength - locksLength[(i + start) % locksLength.Length];
            keys[i] = CreatePin(keyLength + 1, i, -1, keysPos, keyPref);
            keys[i].transform.SetParent(keyParent);
            keys[i].name = "Pin" + i;
            Debug.Log(keyLength);
        }

        isPause = false;
    }

    void Update()
    {
        var uiSize = new Vector2(uiWidth, uiHeight);
        mask.localScale = uiSize;
        mask.localPosition = -uiSize / 2;
        frame.size = uiSize + new Vector2(2, 2);
        frame.transform.localPosition = -uiSize / 2;

        if (!isPause) // �s�����X�N���[��������
        {
            foreach (var pin in locks)
            {
                var scrollSize = hGap * locksCount;
                pin.transform.localScale = new Vector3(wGap * pin.length, hGap);
                pin.pos += Time.deltaTime * scrollSpeed;
                pin.pos %= locksCount;
                pin.transform.localPosition = GetSortedPinPos(pin, 1, -1);
            }

            foreach (var pin in keys)
            {
                var scrollSize = hGap * locksCount;
                pin.transform.localScale = new Vector3(wGap * pin.length, hGap);
                pin.transform.localPosition = GetSortedPinPos(pin, -1, keysPos);
            }
        }

        if (!isPause && Input.GetKeyDown(KeyCode.Space)) // ���̏ƍ�
        {
            isPause = true;

            var offsetY = 0f;
            var offsetX = float.MaxValue;

            bool verify = true;
            foreach (var key in keys)
            {
                var keyY = key.transform.position.y;
                var targets = locks.Select(x => Mathf.Abs(x.transform.position.y - keyY)).ToList();
                var index = targets.IndexOf(targets.Min());

                if (key.length - 1 + locks[index].length != maxLength)
                {
                    verify = false;
                }
                else
                {
                    offsetY = key.transform.position.y - locks[index].transform.position.y; // �������ɂ����g�p���Ȃ�
                }

                //Debug.Log($"{index} {key.length} {locks[index].length}");
            }

            if (verify)
            {
                // ���̏c�ړ��A�j���[�V����
                foreach (var pin in locks)
                {
                    pin.transform.DOLocalMoveY(offsetY, 0.1f).SetRelative();
                }

                transform.DOShakePosition(duration, strength, vibrato).OnComplete(Complete);

                offsetX = uiWidth - wGap * (maxLength + 1); // �������̌��̉��ړ��̑傫���B�s�b�^���Ƃ܂�悤�ɂ���
            }
            else
            {
                foreach (var pin in keys) // ���s���̌��̉��ړ��̑傫���B���[�ȂƂ���Ɉ���������悤�ɂ���
                {
                    var offsetKeys = locks.Where(x => Mathf.Abs(x.transform.position.y - pin.transform.position.y) < hGap).ToList();
                    offsetKeys.ForEach(x => offsetX = Mathf.Min(offsetX, uiWidth - pin.length * wGap - (x.length * wGap)));
                }
            }

            foreach (var pin in keys) // ���̏ƍ��A�j���[�V����
            {
                pin.transform.DOLocalMoveX(offsetX, 0.05f).SetRelative().SetEase(Ease.Linear); // �E�ɓ������B�������E���s������

                if (!verify) // ���s�̏ꍇ���ɖ߂��A�j���[�V�������Đ����ă|�[�Y������
                {
                    DOTween.Sequence(pin)
                   .AppendInterval(0.2f)
                   .Append(pin.transform.DOLocalMoveX(pin.transform.localPosition.x, 0.1f))
                   .OnComplete(() => isPause = false);
                }
            }
        }
    }

    void Complete()
    {
        DOTween.Sequence(mask)
            .Append(DOTween.To(() => uiHeight, x => uiHeight = x, 0, openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => uiWidth, x => uiWidth = x, 0, openDuration).SetEase(Ease.Linear))
            .OnComplete(() => Destroy(gameObject));
    }

    LockPin CreatePin(int Length, float pos, float right, float down, GameObject pinPref)
    {
        var pinSize = Length * wGap;

        var pin = Instantiate(pinPref, transform).GetComponent<LockPin>();
        pin.length = Length;
        pin.pos = pos;
        pin.transform.localScale = new Vector3(pinSize, hGap);

        pin.transform.Translate(0, -pos * hGap, 0); // �c�ɕ��ׂ�
        pin.transform.localPosition = GetSortedPinPos(pin, right, down);

        return pin;
    }


    float GetPinX(LockPin pin, float right) // �c�𑵂���\�[�g
    {
        var pinSize = pin.length * wGap;
        var x = (pinSize - uiWidth) / 2f * -right - uiWidth / 2f;
        return x;
    }

    float GetPinY(LockPin pin, float down) // �c�ɕ��ׂ�
    {
        var y = Mathf.Lerp(-(keysCount / 2f - 1) * hGap, -uiHeight + (keysCount / 2f - 1) * hGap, (down + 1) / 2) - (pin.pos - keysCount / 2) * hGap;
        return y;
    }

    Vector2 GetSortedPinPos(LockPin pin, float right, float down)
    {
        var x = GetPinX(pin, right);
        var y = GetPinY(pin, down);
        return new Vector2(x, y);
    }
}
