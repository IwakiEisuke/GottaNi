using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �e�Q�[���̐e�N���X�i���Ԑ��p�Binterface�ł��������j
/// </summary>
public abstract class GameBase : MonoBehaviour
{
    public Action<GameSectionResult> sendResult;
    public GameSectionResult result;
    public bool isPlaying;

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
    public virtual void EndGame()
    {
        gameObject.SetActive(false);
        SendResult();
    }
}
