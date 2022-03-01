using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Snake : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI leftButtonText, rightButtonText, fireButtonText;

    public Snake Snake { get; private set; }
    public int SnakeIdx => Snake.Snakes.IndexOf(Snake);

    public void Init(Snake snake)
    {
        Snake = snake;
        nameInput.text = snake.Name;
        nameText.color = snake.Color;
        leftButtonText.text = snake.LeftAction.bindings[0].ToDisplayString();
        rightButtonText.text = snake.RightAction.bindings[0].ToDisplayString();
        fireButtonText.text = snake.FireAction.bindings[0].ToDisplayString();
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

    public void Remove()
    {
        Snake.Delete();
        Destroy(gameObject);
    }
}
