using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StatsStack : MonoBehaviour
{
    [Header("Asset References")]
    [SerializeField] UI_SnakeStats statsPrefab;

    [Header("Scene References")]
    [SerializeField] Transform snakeStatsContainer;

    List<UI_SnakeStats> statsStack = new List<UI_SnakeStats>();

    void OnEnable()
    {
        SnakeScore.OnScoresResorted += SortStack;
        BuildStatsStack();
    }

    void OnDisable()
    {
        SnakeScore.OnScoresResorted -= SortStack;
    }

    void BuildStatsStack()
    {
        // clear old stack
        statsStack.Clear();
        foreach(Transform t in snakeStatsContainer)
        {
            Destroy(t.gameObject);
        }

        // build new stack
        foreach(var snake in Snake.AllSnakes)
        {
            UI_SnakeStats snakeStatsInstance = Instantiate(statsPrefab, snakeStatsContainer);
            snakeStatsInstance.Init(snake);
            statsStack.Add(snakeStatsInstance);
        }

        // sort
        SortStack();
    }

    public void SortStack()
    {
        foreach(var stat in statsStack)
        {
            int idx = SnakeScore.SortedScores.IndexOf(stat.Score);
            stat.transform.SetSiblingIndex(idx);
        }
    }
}