using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }

    [Header("Arena")]
    public SO_PlayerPrefsInt ArenaWidth; // pixel
    public SO_PlayerPrefsInt ArenaHeight; // pixel

    [Header("Snake")]
    public SO_PlayerPrefsFloat SnakeSpeed; // 0.1 = 10% of arena width per second
    public SO_PlayerPrefsFloat SnakeTurnRate; // degrees per second
    public SO_PlayerPrefsFloat SnakeThickness; // 0.005 = 0.5% of arena width
    public SO_PlayerPrefsFloat SnakeGapFrequency; // 0.5 = 50% of arena width distance between gaps (yes this name is not entirely accurate)
    public SO_PlayerPrefsFloat SnakeGapWidth; // 3 = 300% of snake thickness

    public float SnakePixelSpeed => SnakeSpeed.Value * ArenaWidth.Value;
    public float SnakePixelThickness => SnakeThickness.Value * ArenaWidth.Value;
    public float SnakePixelGapFrequency => SnakeGapFrequency.Value * ArenaWidth.Value;
    public float SnakePixelGapWidth => SnakeGapWidth.Value * SnakePixelThickness;

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
