using UnityEngine;

public class DeathOnTouch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // When the player enters the trigger calls the Level_Loader funciton in the game manager
        {
            Level_Manager.Reset_Level();
        }
    }
}
