using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float MoveX;
    private float MoveY;
    private bool isGrounded;
    private bool heldJumping;
    public float targetWarmth = 0;
    public float currentWarmth = 0;

    [Header("Movement")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelSlowdown = 3.5f;
    [SerializeField] private float groundCheckOffset = 0.5f;
    [SerializeField] private Vector3 groundCheckSize = new Vector3(0.5f, 0.35f, 1);
    public List<GameObject> PickUpList; // Used for ball count
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

    // Collision
    [SerializeField] private LayerMask groundLayer;
    private bool groundCheck;
    private bool roofCheck;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private SpriteRenderer srScarf;
    private CapsuleCollider2D cc;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        srScarf = scarf.GetComponent<SpriteRenderer>();
        cc = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Play SFX with a slight pitch offset
    /// </summary>
    void PlaySFX(AudioSource sfx)
    {
        if (!sfx.isPlaying)
        {
            sfx.pitch = Random.Range(0.9f, 1.1f);
            sfx.Play();
        }
    }
    /// <summary>
    /// Show/Hide Scarf and Hat
    /// </summary>
    void SetDress(bool active)
    {
        scarf.SetActive(active);
        hat.SetActive(active);
    }

    private IEnumerator JumpCooldown()
    {
        heldJumping = true;
        yield return new WaitForSeconds(0.2f);
        heldJumping = false;
    }

    void Jump()
    {
        PlaySFX(sfxJump);
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    public void RemoveBall(bool cooldown)
    {
        if (PickUpList.Count == 0)
            return;

        bool isBlue = PickUpList[PickUpList.Count - 1].CompareTag("BluePickedUp");
        Destroy(PickUpList[PickUpList.Count - 1]); //destroys gameobject
        PickUpList.RemoveAt(PickUpList.Count - 1); //also remove it from list (avoid null references)

        // Capsule collider align
        cc.offset = new Vector2(cc.offset.x, cc.offset.y + .5f);
        cc.size = new Vector2(cc.size.x, cc.size.y - 1);

        //if no balls left, hide scarf and hat
        if (PickUpList.Count == 0)
            SetDress(false);

        // Blue balls don't drop back as a pickup
        if (isBlue)
            return;

        // Drop pickup
        Vector2 NewPickUpPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1) - 0.1f);

        GameObject NewPickUp = Instantiate(PickUp, NewPickUpPos, Quaternion.identity);
        NewPickUp.GetComponent<PickUp>().Spawned(cooldown, rb.velocity.x); //spawn new ball

        CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y + gameObject.transform.localScale.y / 2);//moves camera look at
    }

    private void Update()
    {
        // Smoothly lerps the target warmth to the current warmth
        currentWarmth = Mathf.Lerp(targetWarmth, currentWarmth, .1f);

        // Gets momentum and moves it
        MoveX = Input.GetAxis("Horizontal") * moveSpeed;
        MoveY = Input.GetAxis("Vertical");

        // Play rolling sfx
        if(isGrounded && Mathf.Abs(rb.velocity.x) > 1) PlaySFX(sfxRoll);
        else sfxRoll.Stop();

        // Melting animations
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

        // Movement animations
        anim.SetBool("Rolling", PickUpList.Count == 0 && MoveX != 0);

        // Flip sprites
        if (MoveX != 0)
        {
            sr.flipX = MoveX < 0;
            srScarf.flipX = sr.flipX;
        }

        // Grounded jump holding
        if (Input.GetButton("Jump") && isGrounded && !heldJumping)
        {
            StartCoroutine(JumpCooldown());
            Jump();
        }

        // Jumping while in the air with balls
        if (Input.GetButtonDown("Jump") && !isGrounded && PickUpList.Count > 0)
        {
            RemoveBall(false);
            Jump();
        }
    }

    private void FixedUpdate()
    {
        // Check for collisions
        groundCheck = null != Physics2D.OverlapBox(transform.position + Vector3.down * (PickUpList.Count + groundCheckOffset), groundCheckSize, 0, groundLayer);
        roofCheck = null != Physics2D.OverlapBox(transform.position + Vector3.up, Vector3.one, 0, groundLayer);

        if (!isGrounded && groundCheck) // If we just landed on the ground, play the landing sound and spawn particles
        {
            PlaySFX(sfxJump);
            Instantiate(GroundParticles, new Vector2(transform.position.x, transform.position.y - PickUpList.Count), Quaternion.identity);
        }
        isGrounded = groundCheck; // Update grounded status

        if (MoveY < 0 && !isGrounded)
            rb.AddForce(new Vector2(0f, MoveY), ForceMode2D.Impulse); // Down to fall

        if(Mathf.Abs(rb.velocity.x) < moveSpeed)
            rb.AddForce(new Vector2(MoveX / accelSlowdown, 0));
    }

    // --- Collisions ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isBlue = collision.gameObject.CompareTag("BluePickUp");
        if ((collision.gameObject.CompareTag("PickUp") || isBlue) && !roofCheck)
        {
            if (collision.gameObject.GetComponent<PickUp>().cooldownDone)
            {
                // Remove ball
                Destroy(collision.gameObject); //romove loose ball
                PickUpList.Add(Instantiate(isBlue ? BluePickedUp : PickedUp, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.transform.localScale.y * (PickUpList.Count + 1)), Quaternion.identity, gameObject.transform)); //instantiate snowball beneath player

                // Update appearance
                SetDress(true);

                // Fix collider and move player up                                                                                                                                                                                                                         // PickUpList[PickUpList.Count-1].tag = "PickedUp"; //add tag (just to be sure)
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.transform.localScale.y); //moves player up
                cc.offset = new Vector2(cc.offset.x, cc.offset.y - .5f); //changes offset so we don't bug into the ground
                cc.size = new Vector2(cc.size.x, cc.size.y + 1); //changes size so we don't bug into the ground

                //moves camera focus
                CameraLookAt.transform.position = new Vector2(CameraLookAt.transform.position.x, CameraLookAt.transform.position.y - gameObject.transform.localScale.y / 2);
            }
        }
    }

    // Gizmos for collider visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.down * (PickUpList.Count + groundCheckOffset), groundCheckSize);
        Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one);
    }
}