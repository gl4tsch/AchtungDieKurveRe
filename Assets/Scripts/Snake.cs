using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Snake
{
    public string Name;
    public Vector2 Position { get; private set; }
    public Vector2 Direction { get; private set; }
    public Color32 Color { get; private set; }
    public float ThicknessModifier { get; private set; } = 0f;
    public float Thickness => Settings.Instance.SnakeThickness + ThicknessModifier;
    public float SpeedModifier { get; private set; } = 0f;
    public float TurnRateModifier { get; private set; } = 0f;

    public InputAction LeftAction { get; private set; }
    public InputAction RightAction { get; private set; }
    public InputAction FireAction { get; private set; }

    public static List<Snake> Snakes = new List<Snake>();

    int turnSign = 0; // 0 => no turn; -1 => clockwise; 1 => counter clockwise

    public Snake()
    {
        Color = Random.ColorHSV();
        Snakes.Add(this);
        Name = "Snake " + Snakes.IndexOf(this);

        // controls
        LeftAction = new InputAction("left", binding: "<Keyboard>/a");
        LeftAction.started += c => turnSign++;
        LeftAction.canceled += c => turnSign--;
        RightAction = new InputAction("right", binding: "<Keyboard>/d");
        RightAction.started += c => turnSign--;
        RightAction.canceled += c => turnSign++;
        FireAction = new InputAction("fire", binding: "<Keyboard>/s");
    }

    public void Spawn(int arenaPixelWidth, int arenaPixelHeight, int borderWidth)
    {
        Position = new Vector2(Random.Range(0 + borderWidth, arenaPixelWidth - borderWidth), Random.Range(0 + borderWidth, arenaPixelHeight - borderWidth));
        Direction = Random.insideUnitCircle.normalized;
        LeftAction.Enable();
        RightAction.Enable();
    }

    public void UpdatePosition()
    {
        float degrees = Settings.Instance.SnakeTurnRate * turnSign * Time.deltaTime;
        Direction = Quaternion.Euler(0, 0, degrees) * Direction;
        Position += Direction * Settings.Instance.SnakeSpeed * Time.deltaTime;
    }

    public void Delete()
    {
        Snakes.Remove(this);
    }

    public struct SnakeData
    {
        public Vector2 prevPos, newPos;
        public float thickness;
        public Vector4 color;
        public Vector4 collision;
    }
}
