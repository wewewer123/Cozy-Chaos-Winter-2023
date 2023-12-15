using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float MoveX;
    private float MoveY;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameObject PickUp;
    [SerializeField] private GameObject PickedUp;
    [SerializeField] private List<GameObject> PickUpList;
    [SerializeField] private GameObject CameraLookAt;
    private Rigidbody2D rb;

    bool isGrounded;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        //gets momentum and moves it
        MoveX = Input.GetAxis("Horizontal") * moveSpeed;
        MoveY = Input.GetAxis("Vertical");
        

        if (Input.GetKeyDown(KeyCode.W) && (isGrounded || PickUpList.Count >= 1)) //check if you can jump
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            if (!isGrounded)
            {
                RemoveBall();
            }
        }
        if (Input.GetKey(KeyCode.S) && isGrounded) rb.AddForce(new Vector2(0f, MoveY), ForceMode2D.Impulse); //go down

        rb.velocity = new Vector2(MoveX, rb.velocity.y);
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y-(1*PickUpList.Count)), Vector2.down, 1.2f); //raycast to check if grounded
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp"))
        {
            Destroy(collision.gameObject); //romove loose ball
            PickUpList.Add(Instantiate(PickedUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity, gameObject.transform)); //instantiate snowball beneath player
            PickUpList[PickUpList.Count-1].tag = "PickedUp"; //add tag (just to be sure)
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.transform.localScale.y); //moves player up
            gameObject.GetComponent<CircleCollider2D>().offset = new Vector2(gameObject.GetComponent<CircleCollider2D>().offset.x, gameObject.GetComponent<CircleCollider2D>().offset.y - gameObject.transform.localScale.y); //changes offset so we don't bug into the ground
            CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y - gameObject.transform.localScale.y / 2); //moves camera look at
        }
    }
    public bool RemoveBall()
    {
        if(PickUpList.Count != 0)
        {
            Destroy(PickUpList[PickUpList.Count - 1]); //destroys gameobject
            PickUpList.RemoveAt(PickUpList.Count - 1); //also remove it from list (for consistency sake

            gameObject.GetComponent<CircleCollider2D>().offset = new Vector2(gameObject.GetComponent<CircleCollider2D>().offset.x, gameObject.GetComponent<CircleCollider2D>().offset.y + gameObject.transform.localScale.y); //undo offset so we don't float

            Instantiate(PickUp, new Vector2(gameObject.transform.position.x-1.5f, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-250,0), Random.Range(-5, 250))); //spawn loose ball back and give it random physics
            CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y + gameObject.transform.localScale.y / 2);//moves camera look at
            return true;
        }
        return false;
    }
}