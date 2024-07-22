using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // REFERENCES
    private Rigidbody2D rb;
    private Animator animator;

    // PROPERTIES
    private float movementInputDirection;
    private int   facingDirection = 1;
    private float jumpTimer;

    [SerializeField][Min(0)] private float movementSpeed = 5f;
    [SerializeField][Min(0)] private float jumpForce = 5f;
    [Space]
    [SerializeField] private Transform groundCheck;
    [SerializeField][Min(0)] private float groundCheckRadius = 1f;
    [SerializeField] private LayerMask groundLayerMask;
    [Space]
    [SerializeField] private Transform wallCheck;
    [SerializeField][Min(0)] private float wallCheckDistance = 1f;
    [SerializeField][Min(0)] private float wallSlidingSpeed = 2f;
    [Space]
    [SerializeField] private float movementForceInAir;
    [SerializeField] private float airDragMultiplier = 0.95f;
    [SerializeField] private float variableJumpHeightMultiplier = 0.5f;
    [Space]
    [SerializeField] private Vector2 wallJumpDirection;
    [SerializeField] private float wallJumpForce;
    [Space]
    [SerializeField] private float jumpTimerSet = 0.15f;


    // BOOLEANS
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isAttemptingToJump;
    private bool canNormalJump;
    private bool canWallJump;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        wallJumpDirection.Normalize();
    }

    private void Update()
    {
        CheckInput();
        CheckMovementDirection();

        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    /// <summary>
    /// Применить движение RigidBody2D
    /// </summary>
    private void ApplyMovement()
    {
        // Если персонаж в свободном падении
        if (!isGrounded && !isWalking && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(movementInputDirection * movementSpeed, rb.velocity.y);
        }

        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlidingSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
            }
        }
    }

    /// <summary>
    /// Проверка нажатий клавиш управления
    /// </summary>
    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || isTouchingWall)
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        // Контроль разной высоты прыжка
        if (Input.GetButtonUp("Jump"))
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
    }

    /// <summary>
    /// Проверка на возможность совершения прыжка
    /// </summary>
    private void CheckIfCanJump()
    {
        if (isWallSliding)
        {
            canWallJump = true;
        }
        else
        {
            canWallJump = false;
        }

        if (isGrounded)
        {
            canNormalJump = true;
        }
        else
        {
            canNormalJump = false;
        }
    }

    /// <summary>
    /// Проверка на возможность скольжения по стене
    /// </summary>
    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection)
            isWallSliding = true;
        else
            isWallSliding = false;
    }

    /// <summary>
    /// Проверка направления движения
    /// </summary>
    private void CheckMovementDirection()
    {
        if ((isFacingRight && movementInputDirection < 0) || (!isFacingRight && movementInputDirection > 0))
            Flip();

        if (movementInputDirection != 0)
            isWalking = true;
        else
            isWalking = false;
    }

    /// <summary>
    /// Проверка соприкосновений с окружением
    /// </summary>
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayerMask);
    }

    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
        }
        
        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            jumpTimer = 0;
            isAttemptingToJump = false;
        }
    }

    private void WallJump()
    {
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            isWallSliding = false;
            Vector2 forceToAdd = new Vector2(wallJumpDirection.x * wallJumpForce * movementInputDirection, wallJumpDirection.y * wallJumpForce);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);

            jumpTimer = 0;
            isAttemptingToJump = false;
        }
    }

    /// <summary>
    /// Развернуть персонажа
    /// </summary>
    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180, 0);
        }
    }

    /// <summary>
    /// Обновление анимаций персонажа
    /// </summary>
    private void UpdateAnimations()
    {
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsWallSliding", isWallSliding);

        animator.SetFloat("YVelocity", rb.velocity.y);
    }

    #region DRAW GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
    #endregion
}
