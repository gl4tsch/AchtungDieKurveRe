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
        if (!viewType.IsSubclassOf(typeof(UI_View)))
        {
            Debug.LogError($"{viewType} is not a subclass of UI_View.");
            return;
        }

        UI_View v = views.FirstOrDefault((v) => v.GetType() == viewType);

        if (v != null)
        {
            // first disable
            foreach (UI_View view in views)
            {
                view.gameObject.SetActive(false);
            }
            // then enable
            v.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"no view of type {viewType} found in {views}.");
        }
    }
}
