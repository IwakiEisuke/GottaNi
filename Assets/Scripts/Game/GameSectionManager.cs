using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�Q�[���Z�N�V�����̌��ʂ̕ێ����A�Q�[���i�s�𐧌䂷��N���X�B
/// </summary>
public class GameSectionManager : ResultSender
{
    GameSectionResult init = new(0, 0, true, 0);
    GameSectionResult result = new(0, 0, true, 0);
    GameBase[] sequence;
    int count = -1;

    /// <summary>
    /// �Q�[���V�[�P���X���������s����B
    /// </summary>
    /// <param name="result"></param>
    void RunSequence(GameSectionResult result)
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
                ChangeState(this.result); // �V�[�P���X������������I�u�U�[�o�[�֒ʒm�B
            }
        }
        else //���s�����炻�̎��_�ŏI������
        {
            EndGame();
            ChangeState(this.result); // ChangeState���s���_�Ŏ��̃Z�N�V���������s����邱�Ƃɒ���
        }
    }

    /// <summary>
    /// �n���ꂽ�Q�[���V�[�P���X���J�n����
    /// </summary>
    /// <param name="newSequence"></param>
    public void StartSection(GameBase[] newSequence)
    {
        sequence = newSequence;
        count = -1;
        result = init;
        RunSequence(result);
    }

    /// <summary>
    /// �e�Q�[�����I��������
    /// </summary>
    public void EndGame()
    {
        foreach (var game in sequence)
        {
            if (game.gameObject.activeSelf)
            {
                game.EndGame();
            }
        }
    }
}
