using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGroupController : MonoBehaviour
{
    [SerializeField] bool doChangeInteractable;
    [SerializeField] List<Button> buttons;

    void Start()
    {
        buttons.AddRange(GetComponentsInChildren<Button>());

        foreach (var button in buttons)
        {
            foreach (var b in buttons)
            {
                button.onClick.AddListener(() => b.enabled = false);
            }

            if (doChangeInteractable) button.onClick.AddListener(() => button.interactable = false); //����鏇�ԂŎ��s�����ς��悤�ł��B�A�j���[�V���������s�����邽�߂ɂ��̏��Ԃ��ێ����Ă�������
        }
    }
}
