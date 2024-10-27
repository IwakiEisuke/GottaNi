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

            //入れる順番で実行順が変わるようです。アニメーションを実行させるためにこの順番を維持してください
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
