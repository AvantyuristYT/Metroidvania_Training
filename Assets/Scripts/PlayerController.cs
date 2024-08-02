using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // REFERENCES
    private Rigidbody2D rigidBody2D;
    private Animator animator;

    // PROPERTIES
    [SerializeField] private float movementSpeed;
    [Space]
    [SerializeField] private float jumpForce;
    [Space]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayerMask;
    [Space]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    [Space]
    [SerializeField] private float wallSlideSpeed;
    [Space]
    [SerializeField] private float movementForceInAir;
    [SerializeField] private float airDragMultiplier;
    [SerializeField] private float variableJumpHeightMultiplier;

    private float movementInputDirection;

    private bool isFacingRight = true;
    private bool isGrounded;
    private bool isWalking;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        //CheckIfWallSliding();

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, rigidBody2D.velocity.y * variableJumpHeightMultiplier);
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
    }

    private void CheckIfWallSliding()
    {
        if (!isGrounded && isTouchingWall && rigidBody2D.velocity.y < 0.01f) // velocity.y чтобы персонаж сначала прыгнул, а потом уже цеплялся
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckMovementDirection()
    {
        if ((isFacingRight && movementInputDirection < 0) || (!isFacingRight && movementInputDirection > 0))
        {
            Flip();
        }

        if (movementInputDirection != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayerMask);
    }

    private void ApplyMovement()
    {
        if (isGrounded)
        {
            rigidBody2D.velocity = new Vector2(movementInputDirection * movementSpeed, rigidBody2D.velocity.y);
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            rigidBody2D.AddForce(forceToAdd);

            if (Mathf.Abs(rigidBody2D.velocity.x) > movementSpeed)
            {
                rigidBody2D.velocity = new Vector2(movementInputDirection * movementSpeed, rigidBody2D.velocity.y);
            }
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x * airDragMultiplier, rigidBody2D.velocity.y);
        }

        if (isWallSliding)
        {
            if (rigidBody2D.velocity.y < -wallSlideSpeed)
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpForce);
        }
    }

    private void Flip()
    {
        if (!isWallSliding)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180, 0);
        }
    }

    private void UpdateAnimations()
    {
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsWallSliding", isWallSliding);
        animator.SetFloat("YVelocity", rigidBody2D.velocity.y);
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
