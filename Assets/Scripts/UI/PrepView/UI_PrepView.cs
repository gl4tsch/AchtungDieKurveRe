using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PrepView : UI_View
{
    [SerializeField] UI_LobbySnake UISnakePrefab;
    [SerializeField] RectTransform SnakesContainer;
    [SerializeField] RectTransform AddSnakeButton;

    List<UI_LobbySnake> uiSnakes = new List<UI_LobbySnake>();

    public void AddSnake()
    {
        UI_LobbySnake uiSnake = Instantiate(UISnakePrefab, SnakesContainer);
        uiSnakes.Add(uiSnake);
        AddSnakeButton.SetAsLastSibling();
        uiSnake.Init(new Snake());
    }

    public void OnPlayButton()
    {
        GameStateMachine.Instance.ChangeState(new ArenaState());
    }
}
