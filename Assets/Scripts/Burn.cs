using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Burn : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement; // Reference to Player script
    [SerializeField] float timeInterval = 2f; // Time between each ball burned

    private IEnumerator BurnTimer()
    {
        // While player has balls burn them
        while (playerMovement.PickUpList.Count > 0)
        {
            playerMovement.RemoveBall();
            yield return new WaitForSeconds(timeInterval);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player")) StartCoroutine(BurnTimer());
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player")) StopAllCoroutines();
    }
}
