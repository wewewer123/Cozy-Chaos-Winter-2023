using UnityEngine;
using UnityEngine.SceneManagement;
public class Level_Manager : MonoBehaviour
{
    public static int Current_Level = 0;
    public static Level_Manager Instance;
    private void Awake()
    {
        //The singletone is created if there isnt one yet
        if (Level_Manager.Instance == null)
        {
            Level_Manager.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);

        }
    }

    static public void Level_Loader(int level)  //You can pass the index number of a level to load it
    {
        if (level > SceneManager.sceneCountInBuildSettings - 1) return; // Change this to load game over screen
        SceneManager.LoadScene(level);
        Current_Level = level;
    }
    static public void Reset_Level() //When called the scene manager reloads the current scene
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}