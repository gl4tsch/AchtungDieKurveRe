using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_LobbySnake : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI leftButtonText, rightButtonText, fireButtonText;
    [SerializeField] TMP_Dropdown abilityDropdown;

    public Snake Snake { get; private set; }
    public int SnakeIdx => Snake.AllSnakes.IndexOf(Snake);

    public void Init(Snake snake)
    {
        Snake = snake;

        nameInput.text = snake.Name;
        nameText.color = snake.Color;

        leftButtonText.text = snake.LeftAction.bindings[0].ToDisplayString();
        rightButtonText.text = snake.RightAction.bindings[0].ToDisplayString();
        fireButtonText.text = snake.FireAction.bindings[0].ToDisplayString();

        abilityDropdown.options = new List<TMP_Dropdown.OptionData>()
        {
             new TMP_Dropdown.OptionData("Eraser"),
             new TMP_Dropdown.OptionData("TBar")
        };
        abilityDropdown.RefreshShownValue();
        //abilityDropdown.onValueChanged.AddListener(SelectAbility);
    }

    public void OnNameInput(string name)
    {
        Snake.Name = name;
    }

    public void RebindLeft()
    {
        leftButtonText.text = "";
        Rebind(Snake.LeftAction, () => leftButtonText.text = Snake.LeftAction.bindings[0].ToDisplayString());
    }

    public void RebindRight()
    {
        rightButtonText.text = "";
        Rebind(Snake.RightAction, () => rightButtonText.text = Snake.RightAction.bindings[0].ToDisplayString());
    }

    public void RebindFire()
    {
        fireButtonText.text = "";
        Rebind(Snake.FireAction, () => fireButtonText.text = Snake.FireAction.bindings[0].ToDisplayString());
    }

    void Rebind(InputAction action, Action completeCallback)
    {
        string oldBinding = action.ToString();

        var rebindOp = action.PerformInteractiveRebinding();
        rebindOp.WithControlsExcluding("Mouse/position");
        rebindOp.WithControlsExcluding("Mouse/delta");
        rebindOp.WithControlsExcluding("DualShock4GamepadHID/systemButton");
        rebindOp.WithControlsExcluding("DualShock4GamepadHID/touchpadButton");
        rebindOp.WithControlsExcluding("DualShock4GamepadHID/dpad");
        rebindOp.WithCancelingThrough("<Keyboard>/escape");

        // Dispose the operation on completion.
        rebindOp.OnComplete(
           operation =>
           {
               Debug.Log($"Rebound '{oldBinding}' to '{operation.selectedControl}'");
               operation.Dispose();
               completeCallback?.Invoke();
           });

        rebindOp.Start();
    }

    public void SelectAbility(int i)
    {
        if(i == 0)
        {
            Snake.Ability = new EraserAbility(Snake);
        }
        else
        {
            Snake.Ability = new TBarAbility(Snake);
        }
    }

    public void Remove()
    {
        Snake.Delete();
        Destroy(gameObject);
    }
}
