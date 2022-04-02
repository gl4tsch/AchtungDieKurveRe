using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SnakeStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pointsLabel;
    [SerializeField] TextMeshProUGUI abilityUses;

    public SnakeScore Score => mySnake.Score;

    Snake mySnake;

    public void Init(Snake snake)
    {
        mySnake = snake;
        pointsLabel.color = mySnake.Color;
        abilityUses.color = mySnake.Color;

        Score.OnScoreChanged += UpdateScore;
        mySnake.Ability.OnUsesChanged += UpdateUses;

        UpdateScore(Score);
        UpdateUses(mySnake.Ability.Uses);
    }

    private void OnDisable()
    {
        if(Score != null)
        {
            Score.OnScoreChanged -= UpdateScore;
        }
        if(mySnake != null && mySnake.Ability != null)
        {
            mySnake.Ability.OnUsesChanged -= UpdateUses;
        }
    }

    private void UpdateScore(SnakeScore score)
    {
        pointsLabel.text = score.Score.ToString();
    }

    void UpdateUses(int uses)
    {
        abilityUses.text = uses.ToString();
    }
}
