using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �e�Q�[���̐e�N���X�i���Ԑ��p�Binterface�ł��������j
/// </summary>
public abstract class GameBase : MonoBehaviour
{
    public Action<GameSectionResult> sendResult;
    protected GameSectionResult result;
    protected bool isPlaying;
    protected bool gameClosed;

    public abstract void StartGame();

    /// <summary>
    /// ���ʂ�ʒm�B�Q�[���V�[�P���X��i�܂���B
    /// </summary>
    public void SendResult()
    {
        sendResult?.Invoke(result);
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    public virtual void CompleteGame()
    {
        isPlaying = false;
        SendResult();
    }

    /// <summary>
    /// �Q�[���Z�N�V�����������A�܂��̓^�C���I�[�o�[���Ɏ��s����֐�
    /// </summary>
    public virtual void PlayClosingAnimation()
    {
        isPlaying = false;
        gameObject.SetActive(false);
    }
}
