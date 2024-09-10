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
    public SpriteRenderer backGround;

    [Header("GameClear AnimationSettings")]
    public float duration;
    public float strength;
    public int vibrato;

    [Header("Score Settings")]
    public int maxAddScore;
    public int AddScore { get; private set; }
    public float multiplier;

    [Header("Chance Settings")]
    public float maxAddChancePoint;
    public float AddChancePoint { get; private set; }

    [Header("Others")]
    public bool gameIsCompleteOnMissed;

    ///<summary>�X�N���[���̒�~�t���O<br></br>�Q�[���̐�������Ƃ͕�</summary>
    [Header("Debug")]
    [SerializeField] bool isScrollPause;

    public event UnityAction OnCompleteAction;

    /// <summary>�Q�[���̐�������</summary>
    bool isClear;

    bool isShaking;

    PinData[] locks, keys;
    Transform mask;

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
            locks[i] = PinData.CreatePin(locksLength[i], i, lockPref, transform);
            locks[i].name = "Pin" + i;
        }

        // �����̃s�����쐬����BpinsLength���璷����͈͂Ŏ����Ă���maxLength�Ƃ̍������߂�
        var start = Random.Range(0, locksLength.Length);
        keys = new PinData[keysCount];
        for (int i = 0; i < keysCount; i++)
        {
            var keyLength = maxLength - locksLength[(i + start) % locksLength.Length];
            keys[i] = PinData.CreatePin(keyLength + 1, i, keyPref, transform);
            keys[i].transform.SetParent(keyParent);
            keys[i].name = "Pin" + i;
        }

        isScrollPause = false;
    }

    void Update()
    {
        {
            var uiSize = new Vector2(uiWidth, uiHeight);
            mask.localScale = uiSize;
            mask.localPosition = -uiSize / 2;
            frame.size = uiSize + new Vector2(2, 2);
            frame.transform.localPosition = -uiSize / 2;
            backGround.size = uiSize;
            backGround.transform.localPosition = -uiSize / 2;
        }

        if (!isShaking)
        {
            // �U���A�j���[�V������ł������Ȃ��悤��
            transform.position = new Vector3(uiWidth / 2 + centerX, uiHeight / 2 + centerY); // UI�̒��S�����킹��
        }

        if (!isScrollPause) 
        {
            // �s�����X�N���[��������
            foreach (var pin in locks)
            {
                pin.AddPos(scrollSpeed, locksCount);
            }

            PinSetPos(locks, 1, -1);
            PinSetPos(keys, -1, keysPos);
        }

        if (!isScrollPause && Input.GetKeyDown(KeyCode.Space)) // ���̏ƍ�
        {
            isScrollPause = true;

            Verify(out var offset, out var hitPinCount);

            SetScore(hitPinCount);

            Animation(offset.x, offset.y);

            if (gameIsCompleteOnMissed || isClear) // ���̃Q�[�����I�������鏈��
            {
                Invoke(nameof(Complete), duration);
            }
        }
    }

    /// <summary>
    /// �����E���s����
    /// </summary>
    /// <param name="offset">x:���̃A�j���[�V��������<br></br>y:���̃A�j���[�V��������</param>
    /// <param name="hitPinCount">���v�����s���̐�</param>
    void Verify(out Vector2 offset, out int hitPinCount)
    {
        offset.x = float.MaxValue; // ���̃A�j���[�V��������
        offset.y = 0;

        hitPinCount = 0;
        isClear = true;
        foreach (var key in keys)
        {
            var keyY = key.transform.position.y;
            var targets = locks.Select(x => Mathf.Abs(x.transform.position.y - keyY)).ToList();
            var index = targets.IndexOf(targets.Min());

            if (key.length - 1 + locks[index].length != maxLength) // ���s
            {
                isClear = false;
            }

            if (offset.y == 0) // ��񏈗������OK
            {
                offset.y = key.transform.position.y - locks[index].transform.position.y;
            }

            var minLengthAtLocksPin = uiWidth - wGap * (key.length + locks[index].length);
            if (minLengthAtLocksPin < offset.x)
            {
                offset.x = minLengthAtLocksPin;
                hitPinCount = 1;
            }
            else if (minLengthAtLocksPin == offset.x)
            {
                hitPinCount++;
            }
        }
    }

    void SetScore(int hitPinCount)
    {
        // score������
        if (isClear)
        {
            AddScore = (int)(maxAddScore * multiplier);
        }
        else
        {
            AddScore = (int)(1f * hitPinCount / keysCount * maxAddScore); // ���v�����s���̊����ŃX�R�A������
        }

        AddChancePoint = 1f * hitPinCount / keysCount * maxAddChancePoint;
    }

    void Animation(float offsetX, float offsetY)
    {
        // �A�j���[�V����
        if (gameIsCompleteOnMissed || isClear)
        {
            // ���̏c�ړ��A�j���[�V����
            foreach (var pin in locks)
            {
                pin.transform.DOLocalMoveY(offsetY, 0.1f).SetRelative();
            }

            if (isClear)
            {
                isShaking = true;
                transform.DOShakePosition(duration, strength, vibrato).OnComplete(() => isShaking = false);
            }

            //offsetX = uiWidth - wGap * (maxLength + 1); // �������̌��̉��ړ��̑傫���B�s�b�^���Ƃ܂�悤�ړ�������ݒ肷��
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
    }

    void Complete() // �I�����A�j���[�V�����Ɖ��_������
    {
        DOTween.Sequence(mask)
            .Append(DOTween.To(() => uiHeight, x => uiHeight = x, 0, openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => uiWidth, x => uiWidth = x, 0, openDuration).SetEase(Ease.Linear))
            .OnComplete(() => {
                OnCompleteAction?.Invoke();
                Destroy(gameObject);
            });
    }

    public void ExitGame()
    {
        OnCompleteAction = null;
        Complete();
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

    void PinSetPos(PinData pin, float right, float down)
    {
        var scrollSize = hGap * locksCount;
        pin.transform.localScale = new Vector3(wGap * pin.length, hGap);
        pin.transform.localPosition = GetSortedPinPos(pin, right, down);
    }

    void PinSetPos(PinData[] pins, float right, float down)
    {
        foreach (var pin in pins)
        {
            PinSetPos(pin, right, down);
        }
    }
}
