using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 6.5f;

    [Header("Player Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    float moveInput = 0;
    float jumpInput = 0;


    Rigidbody2D playerRB2D;

    private void Awake()
    {
        playerRB2D = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            moveInput = data.direction.x;
            jumpInput = data.direction.y;
        }

        Movement();

        Jumping();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
    }

    private void Movement()
    {
        playerRB2D.velocity = new Vector2(moveInput * moveSpeed, playerRB2D.velocity.y);
    }

    private void Jumping()
    {
        if (jumpInput == 1 && IsGrounded())
        {
            playerRB2D.velocity = new Vector2(playerRB2D.velocity.x, 0);
            playerRB2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void SetInputVector(Vector2 inputVector)
    {
        moveInput = inputVector.x;
        jumpInput = inputVector.y;
    }
}
