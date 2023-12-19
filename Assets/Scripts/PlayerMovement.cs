using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeReference] private GameObject BluePickUp;
    [SerializeReference] private GameObject PickedUp;
    [SerializeReference] private GameObject BluePickedUp;
    [SerializeReference] private GameObject CameraLookAt;
    [SerializeReference] private ParticleSystem GroundParticles;
    [SerializeReference] private GameObject scarf;
    [SerializeReference] AudioSource sfxRoll;
    [SerializeReference] AudioSource sfxJump;
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] GroundCheck roofCheck;
    private bool isGrounded;
    private bool canPickupBalls = true;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D cc;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        // Gets momentum and moves it
        MoveX = Input.GetAxis("Horizontal") * moveSpeed;
        MoveY = Input.GetAxis("Vertical");


        if ((rb.velocity.x >= 1 || rb.velocity.x <= -1) && isGrounded && !sfxRoll.isPlaying) sfxRoll.Play();
        else if (rb.velocity.x < 1 && rb.velocity.x > -1 || !isGrounded) sfxRoll.Stop();

        if (MoveX > 0) sr.flipX = false;
        else if (MoveX < 0) sr.flipX = true;


        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && (isGrounded || PickUpList.Count >= 1)) //check if you can jump
        {
            if(!sfxJump.isPlaying) sfxJump.Play();
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            if (!isGrounded) RemoveBall(false);
        }

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isGrounded) rb.AddForce(new Vector2(0f, MoveY * 0.1f), ForceMode2D.Impulse); //go down
    }

    private void FixedUpdate()
    {
        canPickupBalls = !roofCheck.isGrounded; // If roofCheck is touching the ground, you can't pickup balls, otherwise you can

        if (!isGrounded && groundCheck.isGrounded)
        {
            if(!sfxJump.isPlaying) sfxJump.Play();
            Instantiate(GroundParticles, new Vector2(transform.position.x, transform.position.y - PickUpList.Count), Quaternion.identity);
        }
        isGrounded = groundCheck.isGrounded;

        if (-moveSpeed < rb.velocity.x && rb.velocity.x < moveSpeed) rb.AddForce(new Vector2(MoveX / accelSlowdown, 0));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isBlue = collision.gameObject.CompareTag("BluePickUp");

        if ((collision.gameObject.CompareTag("PickUp") || isBlue) && canPickupBalls)
        {
            if (collision.gameObject.GetComponent<PickUp>().cooldownDone)
            {
                Destroy(collision.gameObject); //romove loose ball
                PickUpList.Add(Instantiate(isBlue ? BluePickedUp : PickedUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity, gameObject.transform)); //instantiate snowball beneath player
                scarf.SetActive(true);

                // Fix collider and move player up                                                                                                                                                                                                                         // PickUpList[PickUpList.Count-1].tag = "PickedUp"; //add tag (just to be sure)
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.transform.localScale.y); //moves player up
                cc.offset = new Vector2(cc.offset.x, cc.offset.y - .5f); //changes offset so we don't bug into the ground
                cc.size = new Vector2(cc.size.x, cc.size.y + 1); //changes size so we don't bug into the ground
                //moves camera focus
                CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y - gameObject.transform.localScale.y / 2);
                //moves groundcheck down
                groundCheck.transform.position = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y - gameObject.transform.localScale.y);
            }
        }
    }

    public bool RemoveBall(bool cooldown)
    {
        if (PickUpList.Count != 0)
        {
            bool isBlue = PickUpList[PickUpList.Count - 1].CompareTag("BluePickedUp");
            Destroy(PickUpList[PickUpList.Count - 1]); //destroys gameobject
            PickUpList.RemoveAt(PickUpList.Count - 1); //also remove it from list (for consistency sake

            cc.offset = new Vector2(cc.offset.x, cc.offset.y + .5f); //undo offset so we don't float
            cc.size = new Vector2(cc.size.x, cc.size.y - 1); //undo size so we don't float
            // moves groundcheck back up
            groundCheck.transform.position = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y + gameObject.transform.localScale.y);

            if (PickUpList.Count == 0) scarf.SetActive(false); //if no balls left, hide scarf
            if (isBlue) return true;

            Vector2 NewPickUpPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1) - 0.1f);

            GameObject NewPickUp = Instantiate(PickUp, NewPickUpPos, Quaternion.identity);
            NewPickUp.GetComponent<PickUp>().Spawned(cooldown, rb.velocity.x); //spawn new ball

            CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y + gameObject.transform.localScale.y / 2);//moves camera look at
            return true;
        }
        return false;
    }
}