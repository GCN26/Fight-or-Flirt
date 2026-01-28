using UnityEngine;

public class CharacterMovementScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveInput;

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
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
