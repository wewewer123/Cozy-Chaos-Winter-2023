using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Level_Manager : MonoBehaviour
{
    private static Level_Manager _current;
    public static Level_Manager current { get => _current; }
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
        if (Input.GetKeyDown(KeyCode.P) && SceneManager.GetActiveScene().buildIndex != 0) {
            if (pauseMenu.activeSelf) {
                pauseMenu.SetActive(false);
            } else {
                pauseMenu.SetActive(true);
            }
        }
    }
    static public void Level_Loader(int level)  //You can pass the index number of a level to load it
    {
        if (level > SceneManager.sceneCountInBuildSettings - 1) return; // Change this to load game over screen
        SceneManager.LoadSceneAsync(level);
        Current_Level = level;
    }
    private IEnumerator DeathMenu()
    {
        deathMenu.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        yield return new WaitForSeconds(1.0f);
        deathMenu.SetActive(false);
    }
    static public void Reset_Level() //When called the scene manager reloads the current scene
    {
        _current.StartCoroutine(current.DeathMenu());
    }
}