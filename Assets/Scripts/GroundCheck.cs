using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    public bool isGrounded;
    private void OnTriggerStay2D(Collider2D collision)
    {
        isGrounded = collision != null && groundLayer == (groundLayer | (1 << collision.gameObject.layer));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }
}
