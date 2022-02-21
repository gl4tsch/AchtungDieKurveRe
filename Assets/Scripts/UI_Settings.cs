using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] UI_Snake UISnakePrefab;
    [SerializeField] RectTransform SnakesContainer;
    [SerializeField] RectTransform AddSnakeButton;

    List<UI_Snake> uiSnakes = new List<UI_Snake>();

    public void AddSnake()
    {
        UI_Snake uiSnake = Instantiate(UISnakePrefab, SnakesContainer);
        uiSnakes.Add(uiSnake);
        AddSnakeButton.SetAsLastSibling();
        uiSnake.Init(new Snake());
    }
}
