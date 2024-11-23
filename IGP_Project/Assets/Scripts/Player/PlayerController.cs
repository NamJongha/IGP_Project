using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

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

    private bool isInPortal = false;
    [Networked] private bool isTrigger { get; set; }
    [Networked] private float gravityScale { get; set; }
    [Networked] private bool spriteVisible { get; set; }

    Rigidbody2D playerRB2D;
    PlayerCollisionHandler collisionHandler;
    BoxCollider2D playerCollider;
    SpriteRenderer playerSprite;

    private ChangeDetector changes;

    private void Awake()
    {
        playerRB2D = GetComponent<Rigidbody2D>();
        collisionHandler = GetComponent<PlayerCollisionHandler>();
        playerCollider = GetComponentInChildren<BoxCollider2D>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Spawned()
    {
        base.Spawned();

        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        gravityScale = 9.8f;
        isTrigger = false;
        spriteVisible = true;
        isInPortal = false;
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

        if (isInPortal == false)
        {
            Movement();

            Jumping();

            UsingItem();
        }

        if (Object.HasStateAuthority)
        {
            HandlePortal();
        }

        playerRB2D.gravityScale = gravityScale;
        playerCollider.isTrigger = isTrigger;
        playerSprite.enabled = spriteVisible;
    }

    public override void Render()
    {
        base.Render();
        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(spriteVisible):
                    var spriteReader = GetPropertyReader<bool>(nameof(spriteVisible));
                    var (prevSprite, curSprite) = spriteReader.Read(previousBuffer, currentBuffer);
                    OnSpriteVisibleChagned(prevSprite, curSprite);
                    break;

                case nameof(isTrigger):
                    var triggerReader = GetPropertyReader<bool>(nameof(isTrigger));
                    var (prevTrigger, curTrigger) = triggerReader.Read(previousBuffer, currentBuffer);
                    OnIsTriggerChanged(prevTrigger, curTrigger);
                    break;

                case nameof(gravityScale):
                    var gravityReader = GetPropertyReader<float>(nameof(gravityScale));
                    var (prevGravity, curGravity) = gravityReader.Read(previousBuffer, currentBuffer);
                    OnGravityScaleChanged(prevGravity, curGravity);
                    break;
            }
        }
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

    private void HandlePortal()
    {
        if(collisionHandler.getPortalValue() != 1 && isInPortal == false) //if player is not on the portal
        {
            gravityScale = 9.8f;
            isTrigger = false;
            spriteVisible = true;
            isInPortal = false;
        }

        if (collisionHandler.getPortalValue() == 1 && portalInput == 1)//entering portal
        {
            isInPortal = true;
            gravityScale = 0;
            isTrigger = true;
            spriteVisible = false;
        }

        if(isInPortal && portalInput == 2)
        {
            gravityScale = 9.8f;
            isTrigger = false;
            spriteVisible = true;
            isInPortal = false;
        }
    }

    private void UsingItem()
    {
        if (useItemInput == 1 /*&& hasItem*/)//if player pressed item use key and having item
        {
            //use item
            //get item's properties if it is dash or double jump
        }

        else if (useItemInput == 2 /*&& hasItem*/)
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

    private void OnSpriteVisibleChagned(NetworkBool oldVal, NetworkBool newVal)
    {
        playerSprite.enabled = newVal;
    }

    private void OnGravityScaleChanged(float oldVal, float newVal)
    {
        playerRB2D.gravityScale = newVal;
    }

    private void OnIsTriggerChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        playerCollider.isTrigger = newVal;
    }
}
