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

        distSinceLastGap += Vector2.Distance(prevPos, Position);
    }

    // returns null if there is nothing to draw
    public SnakeDrawData GetSnakeDrawData()
    {
        var arenaWidth = Settings.Instance.ArenaWidth;
        var snakeData = new SnakeDrawData();

        var prevUVPos = prevPos / arenaWidth;
        var newUVPos = Position / arenaWidth;

        snakeData.oldUVPos = prevUVPos;
        snakeData.newUVPos = newUVPos;
        snakeData.thickness = Thickness / Settings.Instance.ArenaWidth;
        snakeData.color = new Vector4(Color.r, Color.g, Color.b, Color.a);
        snakeData.collision = 0;

        return snakeData;
    }

    /// <summary>
    /// gaps and abilities and the like
    /// </summary>
    /// <returns></returns>
    public List<LineDrawData> GetLineDrawData()
    {
        var data = new List<LineDrawData>();

        // Gap
        if (distSinceLastGap >= Settings.Instance.SnakeGapFrequency)
        {
            var arenaWidth = Settings.Instance.ArenaWidth;

            var prevUVPos = (Position - Direction * Settings.Instance.SnakeGapWidth) / arenaWidth;
            var newUVPos = Position / arenaWidth;

            var gapSegment = new LineDrawData();
            gapSegment.oldUVPos = prevUVPos;
            gapSegment.newUVPos = newUVPos;
            gapSegment.thickness = Thickness / Settings.Instance.ArenaWidth;
            gapSegment.color = new Vector4(0, 0, 0, 0);
            data.Add(gapSegment);
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
        public Vector2 oldUVPos, newUVPos;
        public float thickness;
        public Vector4 color;
        public int collision; // treated as bool
    }

    public struct LineDrawData
    {
        public Vector2 oldUVPos, newUVPos;
        public float thickness;
        public Vector4 color;
    }
}
