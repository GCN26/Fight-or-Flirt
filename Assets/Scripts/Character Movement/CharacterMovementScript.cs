using UnityEngine;

public class CharacterMovementScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveInput;
    private Animator animator;
    private bool isFacingRight = true;

    public bool textAllowMove,battleAllowMove;

    private SpriteRenderer spriteRenderer = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (textAllowMove && battleAllowMove)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.z = Input.GetAxisRaw("Vertical");

        }
        else
        {
            moveInput.x = 0;
            moveInput.z = 0;
        }
        if (moveInput.x > 0)
        {
            animator.SetBool("Moving", true);
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            animator.SetBool("Moving", true);
            spriteRenderer.flipX = true;
        }
        if (moveInput.z > 0)
        {
            animator.SetBool("Moving", true);
        }
        else if (moveInput.z < 0)
        {
            animator.SetBool("Moving", true);
        }
        else if (moveInput.x == 0 &&  moveInput.z == 0)
        {
            animator.SetBool("Moving", false);
        }
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
