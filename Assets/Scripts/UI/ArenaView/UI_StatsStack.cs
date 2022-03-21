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
        BuildStatsStack();
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
        foreach(Snake snake in Snake.AllSnakes)
        {
            UI_SnakeStats snakeStatsInstance = Instantiate(statsPrefab, snakeStatsContainer);
            snakeStatsInstance.Init(snake);
            statsStack.Add(snakeStatsInstance);
        }

        // sort
        SortStats();
    }

    public void SortStats()
    {
        statsStack.Sort((a, b) => b.Score.CompareTo(a.Score));

        foreach(var stat in statsStack)
        {
            stat.transform.SetSiblingIndex(statsStack.IndexOf(stat));
        }
    }
}