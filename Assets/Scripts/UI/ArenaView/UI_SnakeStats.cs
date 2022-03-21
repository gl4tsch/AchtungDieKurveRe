using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SnakeStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pointsLabel;

    public int Score => mySnake.Score;

    Snake mySnake;

    public void Init(Snake snake)
    {
        mySnake = snake;
        pointsLabel.color = mySnake.Color;
        mySnake.OnScoreChanged += UpdateScore;
        UpdateScore(mySnake.Score);
    }

    private void OnDisable()
    {
        if(mySnake != null)
        {
            mySnake.OnScoreChanged -= UpdateScore;
        }
    }

    private void UpdateScore(int score)
    {
        pointsLabel.text = score.ToString();
        GetComponentInParent<UI_StatsStack>().SortStats();
    }
}
