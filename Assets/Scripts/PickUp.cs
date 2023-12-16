using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private float cooldown = 5f;
    public bool cooldownDone = true;
    public void Spawned(bool CoolDown)
    {
        cooldownDone = false;
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-250, 250), Random.Range(-250, 0)));
        if (CoolDown)
        {
            StartCoroutine(waitTime());
        }
        else
        {
            cooldownDone = true;
        }
    }
    private IEnumerator waitTime()
    {
        cooldownDone = false;
        yield return new WaitForSeconds(cooldown);
        cooldownDone = true;
    }
}

