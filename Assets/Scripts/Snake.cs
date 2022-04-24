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

    public SnakeScore Score;
    public BaseAbility Ability;

    public float ThicknessModifier { get; private set; } = 1f;
    public float SpeedModifier { get; private set; } = 1f;
    public float TurnRateModifier { get; private set; } = 1f;

    public InputAction LeftAction { get; private set; }
    public InputAction RightAction { get; private set; }
    public InputAction FireAction { get; private set; }

    public static List<Snake> AllSnakes = new List<Snake>(); // this has to go somewhere else at some point
    public static List<Snake> AliveSnakes = new List<Snake>();

    int turnSign = 0; // 0 => no turn; -1 => clockwise; 1 => counter clockwise
    Vector2 prevPos;

    // Gap
    Stack<LineDrawData> gapSegmentBuffer = new Stack<LineDrawData>();
    float distSinceLastGap = 0;

    List<LineDrawData> injectionDrawBuffer = new List<LineDrawData>();

    public Snake()
    {
        Color = UnityEngine.Random.ColorHSV(0,1,0.5f,1,0.5f,1,1,1);
        AllSnakes.Add(this);
        Debug.Log(Color + " exists!");
        Name = "Snake " + AllSnakes.IndexOf(this);
        Ability = new EraserAbility(this);
        Score = new SnakeScore();

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

    public void Spawn(int arenaPixelWidth, int arenaPixelHeight)
    {
        gapSegmentBuffer.Clear();
        injectionDrawBuffer.Clear();

        // Pos
        Position = new Vector2(UnityEngine.Random.Range(0 + Thickness * 2, arenaPixelWidth - Thickness * 2), UnityEngine.Random.Range(0 + Thickness * 2, arenaPixelHeight - Thickness * 2));
        // don't spawn too close to another snake
        float tooClose = 0.2f * arenaPixelWidth;
        int maxTries = 10;
        int currentTry = 0;
        while(currentTry < maxTries)
        {
            float minDist = Mathf.Infinity;
            foreach(var snake in AliveSnakes)
            {
                float dist = Vector2.Distance(Position, snake.Position);
                minDist = Mathf.Min(dist, minDist);
            }
            if(minDist > tooClose)
            {
                break;
            }
            Position = new Vector2(UnityEngine.Random.Range(0 + Thickness * 2, arenaPixelWidth - Thickness * 2), UnityEngine.Random.Range(0 + Thickness * 2, arenaPixelHeight - Thickness * 2));
            currentTry++;
        }

        // Dir
        //Direction = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 center = new Vector2(arenaPixelWidth / 2, arenaPixelHeight / 2);
        Direction = (center - Position).normalized;

        Ability.SetUses(Score.Place);

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
            gapSegment.thickness = (Thickness + 4f) / arenaWidth;
            gapSegment.color = new Vector4(0, 0, 0, 0);
            gapSegment.clipCircle = 1; // clip gap around start

            // check if data can be combined
            // TODO: unspaghetti
            if (gapSegmentBuffer.Count > 0)
            {
                var lastSegment = gapSegmentBuffer.Peek();

                if (Vector2.Angle(lastSegment.UVPosB - lastSegment.UVPosA, newUVPos - prevUVPos) < 0.0001)
                {
                    gapSegmentBuffer.Pop();

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
                // first gap segment
                gapSegment.UVPosA = prevUVPos;
                gapSegment.UVPosB = newUVPos;
            }

            gapSegmentBuffer.Push(gapSegment);
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
            // clip end of gap
            var lastSegment = gapSegmentBuffer.Pop();
            lastSegment.clipCircle = lastSegment.clipCircle == 1 ? 3 : 2;
            gapSegmentBuffer.Push(lastSegment);

            data.AddRange(gapSegmentBuffer);
            gapSegmentBuffer.Clear();
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
        Score.IncreaseScore(AllSnakes.Count - AliveSnakes.Count);
        AliveSnakes.Remove(this);
        Debug.Log(Color + " ded!");
    }

    public void Reset()
    {
        Score.Reset();
        Ability.SetUses(Score.Place);
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
        public int clipCircle; // 0 = no clip; 1 = circle around a; 2 = circle around b; 3 = circle around both
    }
}
