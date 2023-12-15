using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "PickUp";
        GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp")) //why do we do it like this? this would make snowballs deactivate eachother even without player present.
        {
            collision.gameObject.SetActive(false);
        }
    }
}

