using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Snake
{
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }
    public Color32 Color { get; private set; }
    public float Thickness { get; private set; } = 5;
    public float Speed { get; private set; } = 100f;
    public float TurnRate { get; private set; } = 100f;
    public float TurnRadius { get; private set; } = 50f;

    public static List<Snake> Snakes = new List<Snake>();

    int turnSign = 0; // 0 => no turn; -1 => clockwise; 1 => counter clockwise

    public Snake(Color32 color)
    {
        Color = color;

        // controls
        var left = new InputAction("left", binding: "<Keyboard>/a");
        left.started += c => turnSign++;
        left.canceled += c => turnSign--;
        left.Enable();
        var right = new InputAction("right", binding: "<Keyboard>/d");
        right.started += c => turnSign--;
        right.canceled += c => turnSign++;
        right.Enable();

        Snakes.Add(this);
    }

    public void UpdatePosition()
    {
        float degrees = TurnRate * turnSign * Time.deltaTime;
        Vector2 turnCenter = Position + new Vector2(-Direction.x, Direction.y).normalized * TurnRadius;

        Direction = Quaternion.Euler(0, 0, degrees) * Direction;
        Position += Direction * Speed * Time.deltaTime;
    }

    public struct SnakeData
    {
        public Vector2 prevPos, newPos;
        public float thickness;
        public Vector4 color;
        public Vector4 collision;
    }
}
