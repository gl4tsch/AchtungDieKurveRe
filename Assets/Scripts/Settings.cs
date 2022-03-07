using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }

    [Header("Arena")]
    public int ArenaWidth = 1024;
    public int ArenaHeight = 1024;

    [Header("Snake")]
    public float SnakeSpeed = 100f;
    public float SnakeTurnRate = 150f;
    public float SnakeThickness = 5f;
    public float SnakeGapFrequency = 200f; // distance between gaps in pixels
    public float SnakeGapWidth = 20f; // how wide is the gap

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
}
