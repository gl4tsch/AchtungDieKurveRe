using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Snake : StateMachine<Snake>
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

    public float ThicknessModifier { get; set; } = 1f;
    public float SpeedModifier { get; set; } = 1f;
    public float TurnRateModifier { get; set; } = 1f;

    public InputAction LeftAction { get; private set; }
    public InputAction RightAction { get; private set; }
    public InputAction FireAction { get; private set; }

    public static List<Snake> AllSnakes = new List<Snake>(); // this has to go somewhere else at some point
    public static List<Snake> AliveSnakes = new List<Snake>();

    public BaseState AliveState { get; private set; } = new AliveSnakeState();
    public BaseState DeadState { get; private set; } = new DeadSnakeState();

    int turnSign = 0; // 0 => no turn; -1 => clockwise; 1 => counter clockwise
    Vector2 prevPos;

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

        ChangeState(DeadState);
    }

    public void Spawn()
    {
        ChangeState(AliveState);
    }

    public void Kill()
    {
        ChangeState(DeadState);
    }

    public void Reset()
    {
        Score.Reset();
        Ability.SetUses(Score.Place);
    }

    public void Delete()
    {
        if(AliveSnakes.Contains(this))
        {
            AliveSnakes.Remove(this);
        }

        AllSnakes.Remove(this);
        Debug.Log(Color + " deleted!");
    }

    public void Teleport(Vector2 position)
    {
        Position = position;
        prevPos = position;
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

        return data;
    }

    public void InjectLineDrawData(List<LineDrawData> lineDrawData)
    {
        injectionDrawBuffer.AddRange(lineDrawData);
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

        /// <summary>
        /// 0 = no clip; 1 = circle around a; 2 = circle around b; 3 = circle around both
        /// </summary>
        public int clipCircle;
    }
}
