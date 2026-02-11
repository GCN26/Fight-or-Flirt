using UnityEngine;

public class CharacterMovementScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveInput;

    public GameObject quadToFlip;
    private bool isFacingRight = true;

    public bool allowMove;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (allowMove)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.z = Input.GetAxisRaw("Vertical");

        }
        else
        {
            moveInput.x = 0;
            moveInput.z = 0;
        }
        if (moveInput.x < 0 && isFacingRight || moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 playerScale = quadToFlip.transform.localScale;
        playerScale.x *= -1;
        quadToFlip.transform.localScale = playerScale;
    }
}
