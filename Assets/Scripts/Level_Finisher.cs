using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Level_Finisher : MonoBehaviour
{
  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // When the player enters the trigger calls the Level_Loader funciton in the game manager
        {

            Level_Manager.Level_Loader(SceneManager.GetActiveScene().buildIndex + 1);

        }
    }


}
