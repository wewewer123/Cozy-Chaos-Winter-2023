using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    private static LevelManager _current;
    public static LevelManager current { get => _current; }
    public static int Current_Level = 0;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject pauseMenu;
    void Awake()
    {
        if (_current == null)
        {
            _current = this;
            DontDestroyOnLoad(this.gameObject); // This will make the Level_Manager object persist between scenes
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("There shouldn't be more than one LevelManager in scene");
        }
    }
    private void Update() {
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape )) && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().name != "Credits") {
            if (pauseMenu.activeSelf) {
                pauseMenu.SetActive(false);
            } else {
                pauseMenu.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && SceneManager.GetActiveScene().buildIndex != 0) {
            ResetLevel(false);
        }
    }
    static public void LoadLevel(int level)  //You can pass the index number of a level to load it
    {
        if (level > SceneManager.sceneCountInBuildSettings - 1) return; // Change this to load game over screen
        SceneManager.LoadSceneAsync(level);
        Current_Level = level;
    }
    private IEnumerator ReloadLevel()
    {
        deathMenu.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        yield return new WaitForSeconds(1.0f);
        deathMenu.SetActive(false);
    }
    static public void ResetLevel(bool death) //When called the scene manager reloads the current scene with death menu
    {
        if (death) _current.StartCoroutine(current.ReloadLevel());
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}