using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // This is a singleton class
    private static MenuManager _current;
    public static MenuManager current { get => _current; }

    // Event called when setting menu
    public event System.Action<string> onSetMenu;

    [SerializeField] Menu startingMenu;

    // Singleton setup
    void Awake()
    {
        if (_current == null)
            _current = this;
        else
            Debug.LogWarning("There shouldn't be more than one MenuManager in scene");

        
    }

    // Why IEnumerator? -> Has to wait one frame for the Menus to start
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        SetMenu(startingMenu.gameObject.name);
    }

    /// <summary>
    /// When called, calls an event which enables the menu with the specified name
    /// </summary>
    /// <param name="menuName"></param>
    public void SetMenu(string menuName)
    {
        if (onSetMenu != null)
            onSetMenu(menuName);
    }
}
