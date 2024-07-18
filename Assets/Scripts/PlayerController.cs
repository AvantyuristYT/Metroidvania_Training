using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // REFERENCES
    private Rigidbody2D rigidBody;
    private Animator animator;

    [Space]

    // VARIABLES
    private float movementInputDirection;

    [SerializeField][Min(0)] private float movementSpeed = 5f;
    [SerializeField][Min(0)] private float jumpForce = 5f;
    [Space]
    [SerializeField] private Transform groundCheck;
    [SerializeField][Min(0)] private float groundCheckRadius = 1f;
    [SerializeField] private LayerMask groundLayerMask;
    [Space]
    [SerializeField][Min(0)] private int amountOfJumps = 1;
                             private int amountOfJumpsLeft;
    [Space]
    [SerializeField] private Transform wallCheck;
    [SerializeField][Min(0)] private float wallCheckDistance = 1f;
    [SerializeField][Min(0)] private float wallSlidingSpeed = 2f;

    // BOOLEANS
    private bool isFacingRight = true;
    private bool isWalking = false;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;

    private bool canJump = true;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        amountOfJumpsLeft = amountOfJumps;
    }

    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        CheckIfWallSliding();

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
            Jump();
    }

    private void ApplyMovement()
    {
        if (isGrounded)
        {
            rigidBody.velocity = new Vector2(movementInputDirection * movementSpeed, rigidBody.velocity.y);
        }

        if (isWallSliding)
        {
            if (rigidBody.velocity.y < -wallSlidingSpeed)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -wallSlidingSpeed);
            }
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rigidBody.velocity.y < 0.1)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (amountOfJumpsLeft <= 0)
            canJump = false;
        else
            canJump = true;
    }

    private void Jump()
    {
        if (!canJump)
            return;

        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
        amountOfJumpsLeft--;
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayerMask);
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();

        if (movementInputDirection != 0)
            isWalking = true;
        else
            isWalking = false;
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && rigidBody.velocity.y < 0.01)
            isWallSliding = true;
        else
            isWallSliding = false;
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

        animator.SetFloat("YVelocity", rigidBody.velocity.y);
    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
