using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �����������_���ȃs�����X�N���[����������̂ŁA����Ƀ^�C�~���O�����킹�Ĕ��Α��̃s������������͂߂�Ɖ����錮�B
/// </summary>
public class PinLockController : MonoBehaviour
{
    [Header("GameSettings")]
    /// <summary>�����̃s���̐�</summary>
    public int locksCount;
    /// <summary>�����̃s���̐�</summary>
    public int keysCount;
    /// <summary>�ő�̃s���̒���</summary>
    public int maxLength;
    /// <summary>�ŏ��̃s���̒���</summary>
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

    ///<summary>�X�N���[���̒�~�t���O<br></br>�Q�[���̐�������Ƃ͕�</summary>
    [Header("Debug")]
    [SerializeField] bool isScrollPause;

    PinData[] locks, keys;
    Transform mask;
    /// <summary>�Q�[���̐�������</summary>
    bool isClear;

    public event UnityAction OnCompleteAction;
    public int addScore;

    public bool gameIsCompleteOnMissed;

    private void Start()
    {
        transform.position = new Vector3(uiWidth / 2 + centerX, uiHeight / 2 + centerY);

        isScrollPause = true;
        mask = GetComponentInChildren<SpriteMask>().transform;

        // �J�n���A�j���[�V����
        DOTween.Sequence(mask) 
            .Append(DOTween.To(() => 0, x => uiWidth = x, uiWidth, openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => 0, x => uiHeight = x, uiHeight, openDuration).SetEase(Ease.Linear))
            .OnComplete(Init);

        uiWidth = 0; // Tween�J�n�܂ł�1�t���[��?������̂ŁA����܂ł�ui���J���Ȃ��悤��0�ɂ��Ă���
        uiHeight = 0; // uiWidth��Tween�����܂�0�ɂȂ�Ȃ��̂Ő��0�ɂ��Ă���
    }

    public void Init()
    {
        //�����̃s�����쐬����
        var locksLength = new int[locksCount]; // key�̍쐬�Ɏg�p����
        locks = new PinData[locksCount];
        for (int i = 0; i < locksCount; i++)
        {
            locksLength[i] = Random.Range(minLength, maxLength + 1);
            locks[i] = CreatePin(locksLength[i], i, 1, -1, lockPref);
            locks[i].name = "Pin" + i;
        }

        // �����̃s�����쐬����BpinsLength���璷����͈͂Ŏ����Ă���maxLength�Ƃ̍������߂�
        var start = Random.Range(0, locksLength.Length);
        keys = new PinData[keysCount];
        for (int i = 0; i < keysCount; i++)
        {
            var keyLength = maxLength - locksLength[(i + start) % locksLength.Length];
            keys[i] = CreatePin(keyLength + 1, i, -1, keysPos, keyPref);
            keys[i].transform.SetParent(keyParent);
            keys[i].name = "Pin" + i;
            Debug.Log(keyLength);
        }

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
            // �U���A�j���[�V������ł������Ă��܂����߁A�������肪�o�Ă���͏������Ȃ�
            transform.position = new Vector3(uiWidth / 2 + centerX, uiHeight / 2 + centerY); // UI�̒��S�����킹��
        }

        if (!isScrollPause) 
        {
            // �s�����X�N���[��������
            foreach (var pin in locks)
            {
                var scrollSize = hGap * locksCount;
                pin.transform.localScale = new Vector3(wGap * pin.length, hGap);
                pin.pos += Time.deltaTime * scrollSpeed;
                pin.pos %= locksCount;
                pin.transform.localPosition = GetSortedPinPos(pin, 1, -1);
            }

            // �s�����X�N���[��������
            foreach (var pin in keys)
            {
                var scrollSize = hGap * locksCount;
                pin.transform.localScale = new Vector3(wGap * pin.length, hGap);
                pin.transform.localPosition = GetSortedPinPos(pin, -1, keysPos);
            }
        }

        if (!isScrollPause && Input.GetKeyDown(KeyCode.Space)) // ���̏ƍ�
        {
            isScrollPause = true;

            var offsetY = 0f; // ���̃A�j���[�V��������
            var offsetX = float.MaxValue; // ���̃A�j���[�V��������

            isClear = true;
            foreach (var key in keys) // �����E���s����
            {
                var keyY = key.transform.position.y;
                var targets = locks.Select(x => Mathf.Abs(x.transform.position.y - keyY)).ToList();
                var index = targets.IndexOf(targets.Min());

                if (key.length - 1 + locks[index].length != maxLength)
                {
                    isClear = false;
                }
                else
                {
                    offsetY = key.transform.position.y - locks[index].transform.position.y; // �������ɂ����g�p���Ȃ�
                }

                //Debug.Log($"{index} {key.length} {locks[index].length}");
            }

            if (isClear)
            {
                // ���̏c�ړ��A�j���[�V����
                foreach (var pin in locks)
                {
                    pin.transform.DOLocalMoveY(offsetY, 0.1f).SetRelative();
                }

                transform.DOShakePosition(duration, strength, vibrato);

                offsetX = uiWidth - wGap * (maxLength + 1); // �������̌��̉��ړ��̑傫���B�s�b�^���Ƃ܂�悤�ړ�������ݒ肷��
            }
            else
            {
                foreach (var pin in keys) // ���s���̌��̉��ړ��̑傫���B���[�ȂƂ���Ɉ���������悤�ړ�������ݒ肷��
                {
                    var offsetKeys = locks.Where(x => Mathf.Abs(x.transform.position.y - pin.transform.position.y) < hGap).ToList();
                    offsetKeys.ForEach(x => offsetX = Mathf.Min(offsetX, uiWidth - pin.length * wGap - (x.length * wGap)));
                }
            }

            foreach (var pin in keys) // ���̏ƍ��A�j���[�V����
            {
                pin.transform.DOLocalMoveX(offsetX, 0.05f).SetRelative().SetEase(Ease.Linear); // �E�ɓ������B�������E���s������

                if (!gameIsCompleteOnMissed && !isClear) // ���s�̏ꍇ���ɖ߂��A�j���[�V�������Đ����ă|�[�Y������
                {
                    DOTween.Sequence(pin)
                   .AppendInterval(0.2f)
                   .Append(pin.transform.DOLocalMoveX(pin.transform.localPosition.x, 0.1f))
                   .OnComplete(() => isScrollPause = false);
                }
            }

            if (gameIsCompleteOnMissed || isClear) // ���̃Q�[�����I�������鏈��
            {
                DOTween.Sequence()
                    .AppendInterval(duration)
                    .OnComplete(Complete);
            }
        }
    }

    void Complete() // �I�����A�j���[�V�����B�A�j���[�V�������I�����炱�̃Q�[���I�u�W�F�N�g��j������
    {
        DOTween.Sequence(mask)
            .Append(DOTween.To(() => uiHeight, x => uiHeight = x, 0, openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => uiWidth, x => uiWidth = x, 0, openDuration).SetEase(Ease.Linear))
            .OnComplete(() => {
                OnCompleteAction?.Invoke();
                Destroy(gameObject);
            });
    }

    PinData CreatePin(int Length, float pos, float right, float down, GameObject pinPref)
    {
        var pinSize = Length * wGap;
        var pin = Instantiate(pinPref, transform).GetComponent<PinData>();
        pin.length = Length;
        pin.pos = pos;
        pin.transform.localScale = new Vector3(pinSize, hGap);
        pin.transform.localPosition = GetSortedPinPos(pin, right, down);
        return pin;
    }


    float GetPinX(PinData pin, float right) // �c�𑵂���\�[�g
    {
        var pinSize = pin.length * wGap;
        var x = (pinSize - uiWidth) / 2f * -right - uiWidth / 2f;
        return x;
    }

    float GetPinY(PinData pin, float down) // �c�ɕ��ׂ�
    {
        var y = Mathf.Lerp(-(keysCount / 2f - 1) * hGap, -uiHeight + (keysCount / 2f - 1) * hGap, (down + 1) / 2) - (pin.pos - keysCount / 2) * hGap;
        return y;
    }

    Vector2 GetSortedPinPos(PinData pin, float right, float down)
    {
        var x = GetPinX(pin, right);
        var y = GetPinY(pin, down);
        return new Vector2(x, y);
    }
}
