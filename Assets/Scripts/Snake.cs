using System;
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
    public float Thickness => Settings.Instance.SnakeThickness + ThicknessModifier;
    public int Score { get; private set; } = 0;

    public float ThicknessModifier { get; private set; } = 1f;
    public float SpeedModifier { get; private set; } = 1f;
    public float TurnRateModifier { get; private set; } = 1f;

    public InputAction LeftAction { get; private set; }
    public InputAction RightAction { get; private set; }
    public InputAction FireAction { get; private set; }

    // Events
    public event Action<int> OnScoreChanged;

    public static List<Snake> AllSnakes = new List<Snake>(); // this has to go somewhere else at some point
    public static List<Snake> AliveSnakes = new List<Snake>();

    int turnSign = 0; // 0 => no turn; -1 => clockwise; 1 => counter clockwise
    Vector2 prevPos;

    // Gap
    Stack<LineDrawData> gapBuffer = new Stack<LineDrawData>();
    float distSinceLastGap = 0;

    public Snake()
    {
        Color = UnityEngine.Random.ColorHSV(0,1,0.5f,1,0.5f,1,1,1);
        AllSnakes.Add(this);
        Debug.Log(Color + " exists!");
        Name = "Snake " + AllSnakes.IndexOf(this);

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
        Position = new Vector2(UnityEngine.Random.Range(0 + borderWidth, arenaPixelWidth - borderWidth), UnityEngine.Random.Range(0 + borderWidth, arenaPixelHeight - borderWidth));
        Direction = UnityEngine.Random.insideUnitCircle.normalized;
        LeftAction.Enable();
        RightAction.Enable();
        AliveSnakes.Add(this);
        Debug.Log(Color + " alive!");
    }

    public void Update()
    {
        UpdatePosition();
        UpdateGap();
    }

    void UpdatePosition()
    {
        prevPos = Position;
        float degrees = Settings.Instance.SnakeTurnRate * turnSign * Time.deltaTime;
        Direction = Quaternion.Euler(0, 0, degrees) * Direction;
        Position += Direction * Settings.Instance.SnakeSpeed * Time.deltaTime;
    }

    void UpdateGap()
    {
        distSinceLastGap += Vector2.Distance(prevPos, Position);

        if(distSinceLastGap > Settings.Instance.SnakeGapFrequency)
        {
            // add to gap buffer
            var arenaWidth = Settings.Instance.ArenaWidth;

            var prevUVPos = prevPos / arenaWidth; //(Position - Direction * Settings.Instance.SnakeGapWidth) / arenaWidth;
            var newUVPos = Position / arenaWidth;

            var gapSegment = new LineDrawData();
            gapSegment.thickness = Thickness / Settings.Instance.ArenaWidth;
            gapSegment.color = new Vector4(0, 0, 0, 0);

            // check if data can be combined
            // TODO: unspaghetti
            if(gapBuffer.Count > 0)
            {
                var lastSegment = gapBuffer.Peek();

                if (Vector2.Angle(lastSegment.UVPosB - lastSegment.UVPosA, prevUVPos - newUVPos) < 0.01)
                {
                    gapBuffer.Pop();

                    gapSegment.UVPosA = lastSegment.UVPosA;
                    gapSegment.UVPosB = newUVPos;
                }
                else
                {
                    gapSegment.UVPosA = prevUVPos;
                    gapSegment.UVPosB = newUVPos;
                }
            }
            else
            {
                gapSegment.UVPosA = prevUVPos;
                gapSegment.UVPosB = newUVPos;
            }

            gapBuffer.Push(gapSegment);
        }
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

        // Gap end
        if(distSinceLastGap > Settings.Instance.SnakeGapFrequency + Settings.Instance.SnakeGapWidth)
        {
            data.AddRange(gapBuffer);
            gapBuffer.Clear();
            distSinceLastGap -= Settings.Instance.SnakeGapFrequency + Settings.Instance.SnakeGapWidth;
        }

        return data;
    }

    public void Kill()
    {
        Score += AllSnakes.Count - AliveSnakes.Count;
        OnScoreChanged?.Invoke(Score);
        AliveSnakes.Remove(this);
        Debug.Log(Color + " ded!");
    }

    public void Delete()
    {
        AllSnakes.Remove(this);
        Debug.Log(Color + " deleted!");
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
        public Vector2 UVPosA, UVPosB;
        public float thickness;
        public Vector4 color;
    }
}
