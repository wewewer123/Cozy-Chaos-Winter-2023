using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float MoveX;
    private float MoveY;
    [Header("Movement")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelSlowdown = 3.5f;
    public List<GameObject> PickUpList; // Used by burn script for ball count
    [Header("References")]
    [SerializeReference] private GameObject PickUp;
    [SerializeReference] private GameObject PickedUp;
    [SerializeReference] private GameObject CameraLookAt;
    [SerializeReference] private bool isGrounded;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Gets momentum and moves it
        MoveX = Input.GetAxis("Horizontal") * moveSpeed;
        MoveY = Input.GetAxis("Vertical");


        if (Input.GetKeyDown(KeyCode.W) && (isGrounded || PickUpList.Count >= 1)) //check if you can jump
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            if (!isGrounded) RemoveBall(false);
        }

        if (Input.GetKey(KeyCode.S) && isGrounded) rb.AddForce(new Vector2(0f, MoveY), ForceMode2D.Impulse); //go down

        if(-moveSpeed < rb.velocity.x && rb.velocity.x < moveSpeed)
        {
            rb.AddForce(new Vector2(MoveX/accelSlowdown, 0));
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y-(1*PickUpList.Count)), Vector2.down, 1.00025f-0.40f); //raycast to check if grounded
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp"))
        { 
            if(collision.gameObject.GetComponent<PickUp>().cooldownDone)
            {
                Destroy(collision.gameObject); //romove loose ball
                PickUpList.Add(Instantiate(PickedUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity, gameObject.transform)); //instantiate snowball beneath player
                                                                                                                                                                                                                                              // PickUpList[PickUpList.Count-1].tag = "PickedUp"; //add tag (just to be sure)
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.transform.localScale.y); //moves player up
                gameObject.GetComponent<CircleCollider2D>().offset = new Vector2(gameObject.GetComponent<CircleCollider2D>().offset.x, gameObject.GetComponent<CircleCollider2D>().offset.y - gameObject.transform.localScale.y); //changes offset so we don't bug into the ground
                CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y - gameObject.transform.localScale.y / 2); //moves camera look at
            }
        }
    }

    public bool RemoveBall(bool cooldown)
    {
        if(PickUpList.Count != 0)
        {
            Destroy(PickUpList[PickUpList.Count - 1]); //destroys gameobject
            PickUpList.RemoveAt(PickUpList.Count - 1); //also remove it from list (for consistency sake

            gameObject.GetComponent<CircleCollider2D>().offset = new Vector2(gameObject.GetComponent<CircleCollider2D>().offset.x, gameObject.GetComponent<CircleCollider2D>().offset.y + gameObject.transform.localScale.y); //undo offset so we don't float

            Instantiate(PickUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)-0.1f), Quaternion.identity).GetComponent<PickUp>().Spawned(cooldown); //spawn loose ball back and give it random physics
            CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y + gameObject.transform.localScale.y / 2);//moves camera look at
            return true;
        }
        return false;
    }
}