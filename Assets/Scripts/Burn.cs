using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Burn : MonoBehaviour
{
    // Start is called before the first frame update
  
    [SerializeField] PlayerMovement playerMovement;
    public float timeInterval = 2f;
    
    bool runTimer = false;
    private float time;
    void Start()
    {
        time = timeInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.health <= 0) Debug.Log("Game Over wow omg so cool!"); //Then here we'd have a GameOver function call
        //Debug.Log(playerMovement.health);
        if (runTimer) time -= Time.deltaTime;
        if (timeInterval <= 0) time = timeInterval; 

    }

    void OnCollisionEnter2D(Collision2D collider) 
    {
        playerMovement.RemoveBall();
        Debug.Log("started collision");
        
    }

    void OnCollisionStay(Collision collision) { runTimer = true; Debug.Log("is colliding"); }


    // Gets called when the object exits the collision
    void OnCollisionExit(Collision collision) { runTimer = false; Debug.Log("stopped colliding"); }
    


}
