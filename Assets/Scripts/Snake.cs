using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Snake
{
    public string Name;
    public Vector2 Position { get; private set; } // in pixel
    public Vector2 Direction { get; private set; }
    public Color Color { get; private set; }

    public float Speed => Settings.Instance.SnakePixelSpeed * SpeedModifier;
    public float TurnRate => Settings.Instance.SnakeTurnRate.Value * TurnRateModifier;
    public float Thickness => Settings.Instance.SnakePixelThickness * ThicknessModifier;
    public float GapFrequency => Settings.Instance.SnakePixelGapFrequency;
    public float GapWidth => Settings.Instance.SnakePixelGapWidth;

    public int Score { get; private set; } = 0;

    public BaseAbility Ability;

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
    Stack<LineDrawData> gapDrawBuffer = new Stack<LineDrawData>();
    float distSinceLastGap = 0;

    List<LineDrawData> injectionDrawBuffer = new List<LineDrawData>();

    public Snake()
    {
        Color = UnityEngine.Random.ColorHSV(0,1,0.5f,1,0.5f,1,1,1);
        AllSnakes.Add(this);
        Debug.Log(Color + " exists!");
        Name = "Snake " + AllSnakes.IndexOf(this);
        Ability = new EraserAbility(this);

        // controls
        LeftAction = new InputAction("left", binding: "<Keyboard>/a");
        LeftAction.started += c => turnSign++;
        LeftAction.canceled += c => turnSign--;
        RightAction = new InputAction("right", binding: "<Keyboard>/d");
        RightAction.started += c => turnSign--;
        RightAction.canceled += c => turnSign++;
        FireAction = new InputAction("fire", binding: "<Keyboard>/s");
        FireAction.started += c => Ability.Activate();
    }

    public void Spawn(int arenaPixelWidth, int arenaPixelHeight, int borderWidth)
    {
        Position = new Vector2(UnityEngine.Random.Range(0 + borderWidth, arenaPixelWidth - borderWidth), UnityEngine.Random.Range(0 + borderWidth, arenaPixelHeight - borderWidth));
        Direction = UnityEngine.Random.insideUnitCircle.normalized;
        LeftAction.Enable();
        RightAction.Enable();
        FireAction.Enable();
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
        float degrees = TurnRate * turnSign * Time.deltaTime;
        Direction = Quaternion.Euler(0, 0, degrees) * Direction;
        Position += Direction * Speed * Time.deltaTime;
    }

    void UpdateGap()
    {
        distSinceLastGap += Vector2.Distance(prevPos, Position);

        if(distSinceLastGap > GapFrequency)
        {
            // add to gap buffer
            var arenaWidth = Settings.Instance.ArenaWidth.Value;

            var prevUVPos = prevPos / arenaWidth; //(Position - Direction * Settings.Instance.SnakeGapWidth) / arenaWidth;
            var newUVPos = Position / arenaWidth;

            var gapSegment = new LineDrawData();
            gapSegment.thickness = Thickness / arenaWidth;
            gapSegment.color = new Vector4(0, 0, 0, 0);

            // check if data can be combined
            // TODO: unspaghetti
            if(gapDrawBuffer.Count > 0)
            {
                var lastSegment = gapDrawBuffer.Peek();

                if (Vector2.Angle(lastSegment.UVPosB - lastSegment.UVPosA, prevUVPos - newUVPos) < 0.01)
                {
                    gapDrawBuffer.Pop();

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

            gapDrawBuffer.Push(gapSegment);
        }
    }

    public SnakeDrawData GetSnakeDrawData()
    {
        var arenaWidth = Settings.Instance.ArenaWidth.Value;
        var snakeData = new SnakeDrawData();

        var prevUVPos = prevPos / arenaWidth;
        var newUVPos = Position / arenaWidth;

        snakeData.oldUVPos = prevUVPos;
        snakeData.newUVPos = newUVPos;
        snakeData.thickness = Thickness / arenaWidth;
        snakeData.color = new Vector4(Color.r, Color.g, Color.b, Color.a);
        snakeData.collision = 0;

        return snakeData;
    }

    /// <summary>
    /// gaps and abilities and the like
    /// </summary>
    /// <returns>draw data no collision checks should be done with</returns>
    public List<LineDrawData> GetLineDrawData()
    {
        var data = new List<LineDrawData>();

        // injected draw data from e.g. abilities
        data.AddRange(injectionDrawBuffer);
        injectionDrawBuffer.Clear();

        // Gap end
        if(distSinceLastGap > GapFrequency + GapWidth)
        {
            data.AddRange(gapDrawBuffer);
            gapDrawBuffer.Clear();
            distSinceLastGap -= GapFrequency + GapWidth;
        }

        return data;
    }

    public void InjectLineDrawData(List<LineDrawData> lineDrawData)
    {
        injectionDrawBuffer.AddRange(lineDrawData);
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

    /// <summary>
    /// has collision
    /// </summary>
    public struct SnakeDrawData
    {
        public Vector2 oldUVPos, newUVPos;
        public float thickness;
        public Vector4 color;
        public int collision; // treated as bool
    }

    /// <summary>
    /// no collision
    /// </summary>
    public struct LineDrawData
    {
        public Vector2 UVPosA, UVPosB;
        public float thickness;
        public Vector4 color;
    }
}
