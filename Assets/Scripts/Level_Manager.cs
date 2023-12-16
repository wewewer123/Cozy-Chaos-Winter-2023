using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

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

        EditorSceneManager.LoadScene(level);
        Current_Level = level;

    }
    static public void Reset_Level() //When called the scene manager reloads the current scene
    {

        EditorSceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

}