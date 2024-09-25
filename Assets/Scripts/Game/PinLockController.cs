using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �����������_���ȃs�����X�N���[����������̂ŁA����Ƀ^�C�~���O�����킹�Ĕ��Α��̃s������������͂߂�Ɖ����錮�B
/// </summary>
public class PinLockController : ResultSender
{
    [SerializeField] PinLockGameProperties aaa;
    [SerializeField] UnityEvent OnCompleteEvent;
    ///<summary>�X�N���[���̒�~�t���O<br></br>�Q�[���̐�������Ƃ͕�</summary>
    [Header("Debug")]
    [SerializeField] bool isScroll;

    GameSectionResult result;
    PinData[] locks, keys;
    bool isShaking;

    Transform mask;

    private void Start()
    {
        transform.position = new Vector3(aaa.uiWidth / 2 + aaa.centerX, aaa.uiHeight / 2 +  aaa.centerY);

        isScroll = false;
        mask = GetComponentInChildren<SpriteMask>().transform;

        AudioManager.Play(SoundType.OpenGame);

        // �J�n���A�j���[�V����
        DOTween.Sequence(mask)
            .Append(DOTween.To(() => 0, x => aaa.uiWidth = x, aaa.uiWidth, aaa.openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => 0, x => aaa.uiHeight = x, aaa.uiHeight, aaa.openDuration).SetEase(Ease.Linear))
            .OnComplete(InitPin);

        aaa.uiWidth = 0; // Tween�J�n�܂ł�1�t���[��?������̂ŁA����܂ł�ui���J���Ȃ��悤��0�ɂ��Ă���
        aaa.uiHeight = 0; // uiWidth��Tween�����܂�0�ɂȂ�Ȃ��̂Ő��0�ɂ��Ă���
    }

    void Update()
    {
        SetUISize();

        if (!isShaking)
        {
            // �U���A�j���[�V������ł������Ȃ��悤��
            transform.position = new Vector3(aaa.uiWidth / 2 + aaa.centerX, aaa.uiHeight / 2 + aaa.centerY); // UI�̒��S�����킹��
        }

        if (isScroll)
        {
            // �s�����X�N���[��������
            foreach (var pin in locks)
            {
                pin.AddPos(aaa.scrollSpeed, aaa.locksCount);
            }

            PinSetPos(locks, 1, -1);
            PinSetPos(keys, -1, aaa.keysPos);
        }

        if (isScroll && Input.GetKeyDown(KeyCode.Space)) // ���̏ƍ�
        {
            isScroll = false;

            Matching(out var offset, out var hitPinCount);

            SetScore(hitPinCount);

            Animation(offset.x, offset.y);

            if (aaa.gameCloseEvenIfMissing || result.success) // ���̃Q�[�����I�������鏈��
            {
                Invoke(nameof(Complete), aaa.duration);
            }
        }
    }

    public void InitPin()
    {
        //�����̃s�����쐬����
        var locksLength = new int[aaa.locksCount]; // key�̍쐬�Ɏg�p����
        locks = new PinData[aaa.locksCount];
        for (int i = 0; i < aaa.locksCount; i++)
        {
            locksLength[i] = Random.Range(aaa.minLength, aaa.maxLength + 1);
            locks[i] = PinData.CreatePin(locksLength[i], i, aaa.lockPref, transform);
            locks[i].name = "Pin" + i;
        }

        // �����̃s�����쐬����BpinsLength���璷����͈͂Ŏ����Ă���maxLength�Ƃ̍������߂�
        var start = Random.Range(0, locksLength.Length);
        keys = new PinData[aaa.keysCount];
        for (int i = 0; i < aaa.keysCount; i++)
        {
            var keyLength = aaa.maxLength - locksLength[(i + start) % locksLength.Length];
            keys[i] = PinData.CreatePin(keyLength + 1, i, aaa.keyPref, aaa.keyParent);
            keys[i].name = "Pin" + i;
        }

        isScroll = true;
    }

    void SetUISize()
    {
        var uiSize = new Vector2(aaa.uiWidth, aaa.uiHeight);
        mask.localScale = uiSize;
        mask.localPosition = -uiSize / 2;
        aaa.frame.size = uiSize + new Vector2(2, 2);
        aaa.frame.transform.localPosition = -uiSize / 2;
        aaa.backGround.size = uiSize;
        aaa.backGround.material.SetVector("_ScrollSpeed", new Vector2(aaa.backGroundSpeedX, aaa.backGroundSpeedY));
        aaa.backGround.transform.localPosition = -uiSize / 2;
    }

    /// <summary>
    /// �����E���s����
    /// </summary>
    /// <param name="offset">x:���̃A�j���[�V��������<br></br>y:���̃A�j���[�V��������</param>
    /// <param name="hitPinCount">���v�����s���̐�</param>
    void Matching(out Vector2 offset, out int hitPinCount)
    {
        offset.x = float.MaxValue; // ���̃A�j���[�V��������
        offset.y = 0;

        hitPinCount = 0;
        result.success = true;
        foreach (var key in keys)
        {
            var keyY = key.transform.position.y;
            var targets = locks.Select(x => Mathf.Abs(x.transform.position.y - keyY)).ToList();
            var index = targets.IndexOf(targets.Min());

            if (key.length - 1 + locks[index].length != aaa.maxLength) // ���s
            {
                result.success = false;
            }

            if (offset.y == 0) // ��񏈗������OK
            {
                offset.y = key.transform.position.y - locks[index].transform.position.y;
            }

            var minLengthAtLocksPin = aaa.uiWidth - aaa.wGap * (key.length + locks[index].length);
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
        if (result.success)
        {
            result.score = (int)(aaa.maxAddScore * aaa.multiplier);
        }
        else
        {
            result.score = (int)(1f * hitPinCount / aaa.keysCount * aaa.maxAddScore); // ���v�����s���̊����ŃX�R�A������
        }

        result.chancePoint = 1f * hitPinCount / aaa.keysCount * aaa.maxAddChancePoint;
    }

    void Animation(float offsetX, float offsetY)
    {
        var moveDuration = 0.1f;
        // �A�j���[�V����
        if (aaa.gameCloseEvenIfMissing || result.success)
        {

            // ���̏c�ړ��A�j���[�V����
            foreach (var pin in locks)
            {
                pin.transform.DOLocalMoveY(offsetY, moveDuration).SetRelative();
            }

            if (result.success)
            {
                isShaking = true;
                transform.DOShakePosition(aaa.duration, aaa.strength, aaa.vibrato).OnComplete(() => isShaking = false);
            }
        }
        else
        {
            foreach (var pin in keys) // ���s���̌��̉��ړ��̑傫���B���[�ȂƂ���Ɉ���������悤�ړ�������ݒ肷��
            {
                var offsetKeys = locks.Where(x => Mathf.Abs(x.transform.position.y - pin.transform.position.y) < aaa.hGap).ToList();
                offsetKeys.ForEach(x => offsetX = Mathf.Min(offsetX, aaa.uiWidth - pin.length * aaa.wGap - (x.length * aaa.wGap)));
            }
        }

        foreach (var pin in keys) // ���̏ƍ��A�j���[�V����
        {
            pin.transform.DOLocalMoveX(offsetX, moveDuration / 2f).SetRelative().SetEase(Ease.Linear); // �E�ɓ������B�������E���s������

            if (!aaa.gameCloseEvenIfMissing && !result.success) // ���s�̏ꍇ���ɖ߂��A�j���[�V�������Đ����ă|�[�Y������
            {
                DOTween.Sequence(pin)
               .AppendInterval(0.2f)
               .Append(pin.transform.DOLocalMoveX(pin.transform.localPosition.x, 0.1f))
               .OnComplete(() => isScroll = true);
            }
        }

        AudioManager.Play(result.success ? SoundType.MatchingSuccess : SoundType.MatchingFailure);
    }

    void Complete() // �I�����A�j���[�V�����Ɖ��_������
    {
        AudioManager.Play(SoundType.CloseGame);
        OnCompleteEvent.Invoke();

        DOTween.Sequence(mask)
            .Append(DOTween.To(() => aaa.uiHeight, x => aaa.uiHeight = x, 0, aaa.openDuration).SetEase(Ease.Linear))
            .Append(DOTween.To(() => aaa.uiWidth, x => aaa.uiWidth = x, 0, aaa.openDuration).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                ChangeState(result);
            });
    }

    void PinSetPos(PinData[] pins, float right, float down)
    {
        foreach (var pin in pins)
        {
            pin.GetComponent<SpriteRenderer>().size = new Vector3(aaa.wGap * pin.length, aaa.hGap);
            pin.transform.localPosition = GetSortedPinPos(pin, right, down);
        }
    }
    Vector2 GetSortedPinPos(PinData pin, float right, float down)
    {
        var x = GetPinX(pin, right);
        var y = GetPinY(pin, down);
        return new Vector2(x, y);
    }

    float GetPinX(PinData pin, float right) // �c�𑵂���\�[�g
    {
        var pinSize = pin.length * aaa.wGap;
        var x = (pinSize - aaa.uiWidth) / 2f * -right - aaa.uiWidth / 2f;
        return x;
    }

    float GetPinY(PinData pin, float down) // �c�ɕ��ׂ�
    {
        var y = Mathf.Lerp(-(aaa.keysCount / 2f - 1) * aaa.hGap, -aaa.uiHeight + (aaa.keysCount / 2f - 1) * aaa.hGap, (down + 1) / 2) - (pin.pos - aaa.keysCount / 2) * aaa.hGap;
        return y;
    }

    public void ExitGame()
    {
        DOTween.Kill(gameObject);
        Complete();
    }

    public override void ChangeState(GameSectionResult result)
    {
        NotifyObservers(result);
        gameObject.SetActive(false);
    }
}
