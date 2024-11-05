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
    float portalInput = 0;
    float useItemInput = 0;
    float emotionInput = 0;

    bool isInPortal = false;
    bool hasItem = false; //flag if the player has item

    Rigidbody2D playerRB2D;
    PlayerCollisionHandler collisionHandler;

    private void Awake()
    {
        playerRB2D = GetComponent<Rigidbody2D>();
        collisionHandler = GetComponent<PlayerCollisionHandler>();
    }

    //clinets get input data from host
    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            moveInput = data.direction.x;
            jumpInput = data.direction.y;
            portalInput = data.enterPortal;
            useItemInput = data.useItem;
        }

        if (!isInPortal)
        {
            Movement();

            Jumping();

            usingItem();
        }

        onPortal();
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

    private void onPortal()
    {
        //enterPortal: player's input, getPortalValue: is Player is on the portal
        if(portalInput == 1 && collisionHandler.getPortalValue() == 1)
        {
            isInPortal = true;
            this.GetComponent<Rigidbody2D>().gravityScale = 0;
            this.GetComponentInChildren<BoxCollider2D>().enabled = false;
            this.GetComponentInChildren<SpriteRenderer>().enabled = false;

        }

        //exitPortal
        if (portalInput == 2 && isInPortal)
        {
            this.GetComponent<Rigidbody2D>().gravityScale = 9.8f;
            this.GetComponentInChildren<BoxCollider2D>().enabled = true;
            this.GetComponentInChildren<SpriteRenderer>().enabled = true;
            isInPortal = false;
        }
    }

    private void usingItem()
    {
        if (useItemInput == 1 && hasItem)//if player pressed item use key and having item
        {
            //use item
            //get item's properties if it is dash or double jump
        }

        else if (useItemInput == 2 && hasItem)
        {
            //drop item
        }
    }

    private void representEmotion()
    {
        //show different emotion according to emotionInput value
    }

    public void SetInputVector(Vector2 inputVector)
    {
        moveInput = inputVector.x;
        jumpInput = inputVector.y;
    }

    public void SetEnterPortal(float portalPressed)
    {
        portalInput = portalPressed;
    }

    public void SetUseItem(float useItemPressed)
    {
        useItemInput = useItemPressed;
    }

    public void SetEmotion(float emotionNum)
    {
        emotionInput = emotionNum;
    }
}
