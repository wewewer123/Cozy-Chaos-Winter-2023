using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // This is a singleton class
    private static MenuManager _current;
    public static MenuManager current { get => _current; }

    // Event called when setting menu
    public event System.Action<string> onSetMenu;
    [SerializeField] Menu startingMenu;
    [SerializeField] GameObject exitButton;

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
        #if UNITY_WEBGL
            exitButton.SetActive(false);
        #endif
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

    public void StartGame()
    {
        SceneManager.LoadScene(1); // Build index 1 is Level1
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
