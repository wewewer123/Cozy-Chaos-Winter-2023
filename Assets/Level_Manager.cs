using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class Level_Manager : MonoBehaviour
{
  
    public static int Current_Level = 0;
    public static Level_Manager Instance;



    [SerializeField] private int Aux_Var = 0;  //This is just to see if the singletone maintains information inbetween scenes

    private void Awake()
    {
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


    public void Update_Aux_Var(int number)
    { 
        Aux_Var += number;
    }

    static public void Level_Loader(int level)  //You can pass the index number of a level to load it
    {
   
        EditorSceneManager.LoadScene(level);
        Current_Level  = level;

    }
    static public void Reset_Level() //When called the scene manager reloads the current scene
    {
    
        EditorSceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }

    private void Set_Current_Level(int level)
    {
        Current_Level = level;
    }

    //This down here is just for debug , not really important you can delete it or use it
    /// ///////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        Debug_Shit();
    }

    private void Debug_Shit()
    {
            
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            Reset_Level();
        }
    
    }
}
