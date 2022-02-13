using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake
{
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }
    public Color32 Color { get; private set; }
    public float Thickness { get; private set; } = 5;
    public float Speed { get; private set; } = 100f;
    public float TurnRate { get; private set; } = 100f;

    public Snake(Color32 color)
    {
        Color = color;
    }

    /// <summary>
    /// turnSign:
    /// 0 => no turn
    /// 1 => clockwise
    /// -1 => counter clockwise
    /// </summary>
    /// <param name="turnSign"></param>
    public void UpdatePosition(int turnSign)
    {
        float degrees = TurnRate * turnSign * Time.deltaTime;
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
