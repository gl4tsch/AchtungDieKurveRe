using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }

    [Header("Arena")]
    public SO_PlayerPrefsInt ArenaWidth;
    public int ArenaHeight = 1024;

    [Header("Snake")]
    public float SnakeSpeed = 100f;
    public float SnakeTurnRate = 150f;
    public float SnakeThickness = 5f;
    public float SnakeGapFrequency = 200f; // distance between gaps in pixels
    public float SnakeGapWidth = 20f; // how wide is the gap

    private void Awake()
    {
        // Singleton
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}
