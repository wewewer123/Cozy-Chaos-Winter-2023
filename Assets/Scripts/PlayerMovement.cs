using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float MoveX;
    private float MoveY;
    public float JumpForce;
    private Rigidbody2D rb;
     public float SpeedX;
    public float SpeedY;
    bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        JumpForce = 5f;
        SpeedX = 10f;
        SpeedY = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        MoveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(MoveX * SpeedX, rb.velocity.y);
        MoveY = Input.GetAxis("Vertical");
        

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
        rb.velocity = new Vector2(MoveX * SpeedX * JumpForce, rb.velocity.x);
        }
    }
    void FixedUpdate()
    {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
         if (hit.collider != null)
         {
            isGrounded = true;
         }
    }
}
