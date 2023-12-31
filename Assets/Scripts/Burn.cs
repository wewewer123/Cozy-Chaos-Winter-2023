using System.Collections;
using UnityEngine;

public class Burn : MonoBehaviour
{
    private PlayerMovement playerMovement; // Reference to Player script
    [SerializeField] float timeInterval = 2f; // Time between each ball burned

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    private IEnumerator BurnTimer()
    {
        // While player has balls burn them
        while (playerMovement.PickUpList.Count > 0)
        {
            playerMovement.RemoveBall(true);
            yield return new WaitForSeconds(timeInterval);
        }

        // If player has no balls left, stop burning
        yield return new WaitForSeconds(timeInterval);
        if (LevelManager.current == null)
        {
            Debug.LogWarning("Player died, but no LevelManager was found! Start from main menu to get level manager.");

            yield break;
        }
        if (playerMovement.PickUpList.Count == 0) LevelManager.ResetLevel(true);
        else StartCoroutine(BurnTimer());
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player")) StartCoroutine(BurnTimer());
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player")) StopAllCoroutines();
    }
}
