using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScore : MonoBehaviour
{
    public int Score { get; private set; }
    public int Place { get; private set; }

    public static List<SnakeScore> SortedScores = new List<SnakeScore>();

    public static event Action OnScoresResorted;
    public event Action<SnakeScore> OnScoreChanged;

    public SnakeScore()
    {
        Score = 0;
        SortedScores.Add(this);
        SortScoresUpdatePlacing();
    }

    public void IncreaseScore(int value)
    {
        Score += value;
        SortScoresUpdatePlacing();
        OnScoreChanged?.Invoke(this);
    }

    void SortScoresUpdatePlacing()
    {
        // sort
        SortedScores.Sort((a, b) => b.Score - a.Score); // descending
        //SortedScores.Reverse(); // descending

        int place = 1;
        // this goes from first to last
        for(int i = 0; i < SortedScores.Count; i++)
        {
            // increase place if score is not the same as the one before
            if(i > 0 && SortedScores[i-1].Score > SortedScores[i].Score)
            {
                place++;
            }

            SortedScores[i].Place = place;
        }

        OnScoresResorted?.Invoke();
    }
}
