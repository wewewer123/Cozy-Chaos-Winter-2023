using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float MoveX;
    private float MoveY;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float moveSpeed = 7f;
    private Rigidbody2D rb;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
        }
    }
}