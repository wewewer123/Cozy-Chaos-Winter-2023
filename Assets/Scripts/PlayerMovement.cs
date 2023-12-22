using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float MoveX;
    private float MoveY;
    private bool OnJumpCooldown;
    private bool isGrounded;
    public float targetWarmth = 0;
    private float currentWarmth = 0;

    [Header("Movement")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelSlowdown = 3.5f;
    [SerializeField] private float jumpCooldown = 0.25f;
    public List<GameObject> PickUpList; // Used by burn script for ball count
    [Header("References")]
    [SerializeReference] private GameObject PickUp;
    [SerializeReference] private GameObject BluePickUp;
    [SerializeReference] private GameObject PickedUp;
    [SerializeReference] private GameObject BluePickedUp;
    [SerializeReference] private GameObject CameraLookAt;
    [SerializeReference] private ParticleSystem GroundParticles;
    [SerializeReference] private GameObject scarf;
    [SerializeReference] private GameObject hat;
    [SerializeReference] AudioSource sfxRoll;
    [SerializeReference] AudioSource sfxJump;
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] GroundCheck roofCheck;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D cc;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }

    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        OnJumpCooldown = false;
    }

    private void Update()
    {
        currentWarmth = Mathf.Lerp(targetWarmth, currentWarmth, .1f); // Smoothly lerps the target warmth to the current warmth
        // Gets momentum and moves it
        MoveX = Input.GetAxis("Horizontal") * moveSpeed;
        MoveY = Input.GetAxis("Vertical");

        if ((rb.velocity.x >= 1 || rb.velocity.x <= -1) && isGrounded && !sfxRoll.isPlaying) sfxRoll.Play();
        else if (rb.velocity.x < 1 && rb.velocity.x > -1 || !isGrounded) sfxRoll.Stop();

        if (currentWarmth > 0)
        {
            anim.speed = currentWarmth * .25f;
            anim.SetBool("Melting", true);
        }
        else
        {
            anim.speed = 1;
            anim.SetBool("Melting", false);
        }

        if (MoveX >= 1 || MoveX <= -1) anim.SetBool("Rolling", PickUpList.Count == 0);
        else anim.SetBool("Rolling", false);

        if (MoveX > 0) sr.flipX = false;
        else if (MoveX < 0) sr.flipX = true;

        // Grounded jump holding
        if (Input.GetButton("Jump") && isGrounded && !OnJumpCooldown)
        {
            OnJumpCooldown = true;
            StartCoroutine(JumpCooldown());
            if(!sfxJump.isPlaying) sfxJump.Play();
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        if (Input.GetButtonDown("Jump") && !isGrounded && PickUpList.Count > 0) // Jumping while in the air with balls
        {
            if(!sfxJump.isPlaying) sfxJump.Play();
            RemoveBall(false);
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (!isGrounded && groundCheck.isGrounded) // If we just landed on the ground, play the landing sound and spawn particles
        {
            if(!sfxJump.isPlaying) sfxJump.Play();
            Instantiate(GroundParticles, new Vector2(transform.position.x, transform.position.y - PickUpList.Count), Quaternion.identity);
        }
        isGrounded = groundCheck.isGrounded; // Update grounded status

        if (MoveY < 0 && !isGrounded) rb.AddForce(new Vector2(0f, MoveY), ForceMode2D.Impulse); // Down to fall

        if (-moveSpeed < rb.velocity.x && rb.velocity.x < moveSpeed) rb.AddForce(new Vector2(MoveX / accelSlowdown, 0));
    }

    private bool canPickupBalls() => !roofCheck.isGrounded;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isBlue = collision.gameObject.CompareTag("BluePickUp");

        if ((collision.gameObject.CompareTag("PickUp") || isBlue) && canPickupBalls())
        {
            if (collision.gameObject.GetComponent<PickUp>().cooldownDone)
            {
                Destroy(collision.gameObject); //romove loose ball
                PickUpList.Add(Instantiate(isBlue ? BluePickedUp : PickedUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity, gameObject.transform)); //instantiate snowball beneath player
                scarf.SetActive(true);
                hat.SetActive(true);

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

            if (PickUpList.Count == 0) { scarf.SetActive(false); hat.SetActive(false); } //if no balls left, hide scarf and hat
            if (isBlue) return true; // Blue balls don't drop back as a pickup

            Vector2 NewPickUpPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1) - 0.1f);

            GameObject NewPickUp = Instantiate(PickUp, NewPickUpPos, Quaternion.identity);
            NewPickUp.GetComponent<PickUp>().Spawned(cooldown, rb.velocity.x); //spawn new ball

            CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y + gameObject.transform.localScale.y / 2);//moves camera look at
            return true;
        }
        return false;
    }
}