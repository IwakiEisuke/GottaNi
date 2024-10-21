using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�Q�[���Z�N�V�����̌��ʂ̕ێ����A�Q�[���i�s�𐧌䂷��N���X�B
/// </summary>
public class GameSectionManager : ResultSender
{
    [SerializeField] GameSpawnerBase spawner;
    [SerializeField] float spacing;
    [SerializeField] float offsetY;
    [SerializeField] float startDelay;
    [SerializeField] float endDelay;

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
        this.result += result;

        if (this.result.success) // �O�Ƀv���C���ꂽ�Q�[���������������H
        {
            count++;
            if (count < sequence.Length)
            {
                sequence[count].StartGame();
                sequence[count].sendResult = RunSequence; // �Q�[�����v���C���ꂽ��A�Q�[�����ł��̃��\�b�h���Ăяo��
            }
            else
            {
                StartCoroutine(EndGame(startDelay, endDelay)); // �V�[�P���X������������I�u�U�[�o�[�֒ʒm�B
            }
        }
        else //���s�����炻�̎��_�ŏI������
        {
            StartCoroutine(EndGame(startDelay, endDelay));
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
            var vec = new Vector3();
            // ���Ԋu�ŉ����тɂ���
            vec.x = (i - ((newSequence.Length - 1) / 2f)) * spacing;
            // �c�ɂ��炷
            vec.y = offsetY;

            newSequence[i].transform.position += vec;
        }

        sequence = newSequence;
        count = -1;
        result = init;
        RunSequence(result);
    }

    /// <summary>
    /// �e�Q�[�����I��������
    /// </summary>
    private IEnumerator EndGame(float startDelay, float endDelay)
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
        StartCoroutine(EndGame(startDelay, endDelay));
    }
}
