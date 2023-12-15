using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Menu : MonoBehaviour
{
    CanvasGroup cg;

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
        MenuManager.current.onSetMenu += OnSetMenu; // Subscribe to event
    }

    void OnSetMenu(string name)
    {
        // TODO: Add animations via tweening library (to simplify it)
        // Set alpha to 1 when menu is opened and disable raycast blocking when closed
        bool itsMe = name == gameObject.name;
        cg.blocksRaycasts = itsMe;
        if (itsMe)
            cg.alpha = 1;
        else
            cg.alpha = 0;
    }
}
