using UnityEngine;
using UnityEngine.SceneManagement;
public class Level_Finisher : MonoBehaviour
{
    [SerializeField] Sprite CabinLit; // Reference to the lit cabin sprite
    private SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // When the player enters the trigger calls the Level_Loader funciton in the game manager
        {
            collision.gameObject.SetActive(false);
            sr.sprite = CabinLit;
            Level_Manager.Level_Loader(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
