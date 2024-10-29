using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�Q�[���Z�N�V�����̌��ʂ�ێ����A�Q�[���i�s�𐧌䂷��N���X�B
/// </summary>
public class GameSectionManager : ResultSender
{
    [SerializeField] GameSpawnerBase spawner;
    [SerializeField] float spacing;
    [SerializeField] float offsetY;

    [SerializeField] float openDelay;
    [SerializeField] float closeStartDelay;
    [SerializeField] float closeEndDelay;

    GameSectionResult init = new(0, 0, true, 0);
    GameSectionResult result = new(0, 0, true, 0);
    GameBase[] sequence;
    int count = -1;

    /// <summary>
    /// �Q�[���V�[�P���X���������s����B
    /// </summary>
    /// <param name="result"></param>
    private void RunSequence(GameSectionResult result)
    {
        if (this.result.success) // �O�Ƀv���C���ꂽ�Q�[���������������H
        {
            count++;
            if (count < sequence.Length)
            {
                sequence[count].StartGame();
                sequence[count].runNext = RunSequence; // �Q�[�����v���C���ꂽ��A�Q�[�����ł��̃��\�b�h���Ăяo��
                sequence[count].sendResult = AddResult;
            }
            else
            {
                StartCoroutine(EndSection(closeStartDelay, closeEndDelay)); // �V�[�P���X������������I�u�U�[�o�[�֒ʒm�B
            }
        }
        else //���s�����炻�̎��_�ŏI������
        {
            StartCoroutine(EndSection(closeStartDelay, closeEndDelay));
        }
    }

    private void AddResult(GameSectionResult result)
    {
        this.result += result;

        if (!this.result.success || count == sequence.Length - 1)
        {
            SendCurrentResult(this.result);
        }
    }

    /// <summary>
    /// �V���ȃQ�[���V�[�P���X���J�n����
    /// </summary>
    [ContextMenu("StartSection")]
    public void StartSection()
    {
        var newSequence = spawner.CreateSequence();

        foreach (var game in newSequence)
        {
            game.transform.parent = transform;
        }

        for (int i = 0; i < newSequence.Length; i++)
        {
            var vec = new Vector3
            {
                // ���Ԋu�ŉ����тɂ���
                x = (i - ((newSequence.Length - 1) / 2f)) * spacing,
                // �c�ɂ��炷
                y = offsetY
            };

            newSequence[i].transform.position += vec;
        }

        sequence = newSequence;
        count = -1;
        result = init;
        StartCoroutine(OpenSection(openDelay));
    }

    private IEnumerator OpenSection(float delay)
    {
        yield return new WaitForSeconds(delay);

        RunSequence(result);
    }

    /// <summary>
    /// �e�Q�[�����I��������
    /// </summary>
    private IEnumerator EndSection(float startDelay, float endDelay)
    {
        yield return new WaitForSeconds(startDelay);

        foreach (var game in sequence)
        {
            if (game.gameObject.activeSelf)
            {
                game.PlayClosingAnimation();
            }
        }

        yield return new WaitForSeconds(endDelay);

        ChangeState(this.result); // ChangeState���s���_�Ŏ��̃Z�N�V���������s����邱�Ƃɒ���
    }

    /// <summary>
    /// �C�x���g���ŊO����Ăяo���p
    /// </summary>
    public void ExecuteEndGame()
    {
        StartCoroutine(EndSection(closeStartDelay, closeEndDelay));
    }
}
