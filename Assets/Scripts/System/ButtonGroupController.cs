using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGroupController : MonoBehaviour
{
    [SerializeField] bool doChangeInteractable;
    [SerializeField] float openDuration;
    [SerializeField] List<Button> buttons;

    void Start()
    {
        buttons.AddRange(GetComponentsInChildren<Button>());

        foreach (var button in buttons)
        {
            button.enabled = false;

            foreach (var b in buttons)
            {
                button.onClick.AddListener(() => b.enabled = false);
            }

            //����鏇�ԂŎ��s�����ς��悤�ł��B�A�j���[�V���������s�����邽�߂ɂ��̏��Ԃ��ێ����Ă�������
            if (doChangeInteractable) button.onClick.AddListener(() => button.interactable = false);
        }

        Invoke(nameof(EnableButtons), openDuration);
    }

    void EnableButtons()
    {
        foreach (var b in buttons)
        {
            b.enabled = true;
        }
    }
}
