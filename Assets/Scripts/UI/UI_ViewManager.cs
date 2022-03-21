using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_ViewManager : MonoBehaviour
{
    List<UI_View> views;

    void Awake()
    {
        views = new List<UI_View>(GetComponentsInChildren<UI_View>(true));
    }

    public void ChangeView(System.Type viewType)
    {
        // first disable
        foreach(UI_View view in views)
        {
            view.gameObject.SetActive(false);
        }

        // then enable
        UI_View v = views.FirstOrDefault((v) => v.GetType() == viewType);
        if (v != null)
        {
            v.gameObject.SetActive(true);
        }
    }
}
