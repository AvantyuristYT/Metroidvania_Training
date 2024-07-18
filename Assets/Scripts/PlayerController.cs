using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // REFERENCES
    private Rigidbody2D rigidBody;

    // VARIABLES
    private float movementInputDirection;

    [SerializeField][Min(0)] private float movementSpeed = 5f;
    [SerializeField][Min(0)] private float jumpForce = 5f;

    // BOOLEANS
    private bool isFacingRight = true;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            Jump();
    }

    private void ApplyMovement()
    {
        rigidBody.velocity = new Vector2(movementInputDirection * movementSpeed, rigidBody.velocity.y);
    }

    private void Jump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
    }


    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }
}
