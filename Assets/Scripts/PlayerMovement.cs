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
    private Rigidbody2D rb;
    public bool Test;

    bool isGrounded;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        MoveX = Input.GetAxis("Horizontal") * moveSpeed;
        MoveY = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.W) && isGrounded) rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        if (Input.GetKey(KeyCode.S) && isGrounded) rb.AddForce(new Vector2(0f, MoveY), ForceMode2D.Impulse);

        rb.velocity = new Vector2(MoveX, rb.velocity.y);
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance: 1.2f);

        if (hit.collider != null) isGrounded = true;
        else isGrounded = false;


        if(Test == true)
        {
            RemoveBall();
            Test = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp"))
        {
            Destroy(collision.gameObject);
            PickUpList.Add(Instantiate(PickedUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity, gameObject.transform));
            PickUpList[PickUpList.Count-1].tag = "PickedUp";
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.transform.localScale.y);
            gameObject.GetComponent<CircleCollider2D>().offset = new Vector2(gameObject.GetComponent<CircleCollider2D>().offset.x, gameObject.GetComponent<CircleCollider2D>().offset.y - gameObject.transform.localScale.y);
        }
    }
    public void RemoveBall()
    {
        if(PickUpList.Count != 0)
        {
            Destroy(PickUpList[PickUpList.Count - 1]);
            PickUpList.RemoveAt(PickUpList.Count - 1);

            gameObject.GetComponent<CircleCollider2D>().offset = new Vector2(gameObject.GetComponent<CircleCollider2D>().offset.x, gameObject.GetComponent<CircleCollider2D>().offset.y + gameObject.transform.localScale.y);

            Instantiate(PickUp, new Vector2(gameObject.transform.position.x-1.5f, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-250,0), Random.Range(-5, 250)));
        }
    }
}