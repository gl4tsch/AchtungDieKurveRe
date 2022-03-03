using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Snake
{
    public string Name;
    public Vector2 Position { get; private set; }
    public Vector2 Direction { get; private set; }
    public Color Color { get; private set; }
    public float ThicknessModifier { get; private set; } = 0f;
    public float Thickness => Settings.Instance.SnakeThickness + ThicknessModifier;
    public float SpeedModifier { get; private set; } = 0f;
    public float TurnRateModifier { get; private set; } = 0f;

    public InputAction LeftAction { get; private set; }
    public InputAction RightAction { get; private set; }
    public InputAction FireAction { get; private set; }

    public static List<Snake> Snakes = new List<Snake>(); // this has to go somewhere else at some point

    int turnSign = 0; // 0 => no turn; -1 => clockwise; 1 => counter clockwise
    Vector2 prevPos;
    float distSinceLastGap = 0;
    bool gap => distSinceLastGap > Settings.Instance.SnakeGapFrequency;

    public Snake()
    {
        Color = Random.ColorHSV(0,1,0.5f,1,0.5f,1,1,1);
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
        prevPos = Position;
        float degrees = Settings.Instance.SnakeTurnRate * turnSign * Time.deltaTime;
        Direction = Quaternion.Euler(0, 0, degrees) * Direction;
        Position += Direction * Settings.Instance.SnakeSpeed * Time.deltaTime;

        // Gap
        distSinceLastGap += Vector2.Distance(prevPos, Position);
    }

    // returns null if there is nothing to draw
    public SnakeDrawData GetDrawData()
    {
        var arenaWidth = Settings.Instance.ArenaWidth;
        var gapFreq = Settings.Instance.SnakeGapFrequency;
        var gapWidth = Settings.Instance.SnakeGapWidth;

        var data = new Snake.SnakeDrawData();

        var prevDataPos = prevPos / arenaWidth;
        var newDataPos = Position / arenaWidth;

        data.oldPos = prevDataPos;
        data.newPos = newDataPos;
        data.thickness = Thickness / Settings.Instance.ArenaWidth;

        data.color = new Vector4(Color.r, Color.g, Color.b, gap ? 0 : Color.a);

        // close gap
        if(distSinceLastGap > gapFreq + gapWidth)
        {
            distSinceLastGap -= Settings.Instance.SnakeGapFrequency + Settings.Instance.SnakeGapWidth;
        }

        return data;
    }

    public void Delete()
    {
        Snakes.Remove(this);
    }

    public struct SnakeDrawData
    {
        public Vector2 oldPos, newPos;
        public float thickness;
        public Vector4 color;
        public Vector4 collision;
    }
}
