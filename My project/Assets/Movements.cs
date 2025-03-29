using UnityEngine;

public class Movements : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Jumping Parameters")]
    public float jumpForce = 10f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Collider2D collider2d;
    private bool isGrounded;
    private float originalXScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
        originalXScale = transform.localScale.x;
    }
    
    private void Update()
    {
        CheckGrounded();
        HandleSideViewMovement();
        if (Input.GetButtonDown("Jump"))  //space
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            // TryJump();
        }
    }

    private void HandleSideViewMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float speed = walkSpeed;
        Move(horizontalInput, speed);
        FlipCharacter(horizontalInput);
    }

    private void HandleTopDownMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float speed = walkSpeed;
        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        movement.Normalize(); // Ensures that diagonal movement isn't faster

        FlipCharacter(horizontalInput);
    }

    private void FlipCharacter(float horizontalInput)
    {
        // Only flip if there is horizontal input
        if (horizontalInput > 0)
            Flip(true);
        else if (horizontalInput < 0)
            Flip(false);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.IsTouchingLayers(collider2d, groundLayer);
    }
    
    void Flip(bool facingRight)
    {
        // Instead of forcing scale.x to 1 or -1,
        // multiply the original x scale by 1 or -1 to preserve size.
        Vector3 scale = transform.localScale;
        scale.x = originalXScale * (facingRight ? 1 : -1);
        transform.localScale = scale;
    }

    private void Move(float horizontalInput, float speed)
    {
        // Removed isGrounded check for testing purposes.
        Vector2 moveVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        rb.linearVelocity = moveVelocity;
    }

    private void TryJump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
}
