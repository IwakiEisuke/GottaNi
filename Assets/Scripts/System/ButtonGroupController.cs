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

            if (doChangeInteractable) button.onClick.AddListener(() => button.interactable = false); //入れる順番で実行順が変わるようです。アニメーションを実行させるためにこの順番を維持してください
        }
    }
}
