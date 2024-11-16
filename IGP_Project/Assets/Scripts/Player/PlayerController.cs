using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 6.5f;
    public float dashForce = 5f;

    [Header("Player Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    //player input value
    float moveInput = 0;
    float jumpInput = 0;
    float portalInput = 0;
    float useItemInput = 0;
    float emotionInput = 0;

    private bool isInPortal = false;
    private int itemCode = -1; //None: -1, double jump: 0, dash: 1, weapon: 2
    private bool isMovable = true;//for dash, if it is false, player can't controll character
    private float lastDirection = 0;//for dash, save the last direction that player looked(moved)

    //parameters below are for item delay(prevent infinite usage of item)
    private bool usedJump = false;
    private bool usedDash = false;

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

        //detect change of object's property
        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        //set initial properties
        gravityScale = 9.8f;
        isTrigger = false;
        spriteVisible = true;

        isInPortal = false;
        itemCode = -1;
        isMovable = true;
        lastDirection = 0;
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

        if(moveInput != 0)
        {
            lastDirection = moveInput;
        }

        if (Object.HasStateAuthority)
        {
            HandlePortal();

            if (isInPortal == false)
            {
                if (isMovable)//player can move only when movable(when dash, movable is disabled)
                {
                    Movement();

                    Jumping();

                    UsingItem();
                }
            }
        }

        playerRB2D.gravityScale = gravityScale;
        playerCollider.isTrigger = isTrigger;
        playerSprite.enabled = spriteVisible;

        Debug.Log(itemCode);
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
        //if (itemCode != 0)//default jump
        //{
            if (jumpInput == 1 && IsGrounded())
            {
                playerRB2D.velocity = new Vector2(playerRB2D.velocity.x, 0);
                playerRB2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
       //}

        //it was for when we use GetKeyDown() for getting input, but GetKeyDown() function causes key-input ignore because of the difference in update frame between Update function and FixedUpdateNetwork function
        //else if (itemCode == 0)//have double jump item
        //{
        //    if (IsGrounded())
        //    {
        //        secondJump = true;
        //    }

        //    if (jumpInput == 1)
        //    {
        //        if (IsGrounded()) { 
        //            Debug.Log(secondJump);
        //            playerRB2D.velocity = new Vector2(playerRB2D.velocity.x, 0);
        //            playerRB2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //        }

        //        if(!IsGrounded() && secondJump)
        //        {
        //            Debug.Log("double jump");
        //            secondJump = false;
        //            playerRB2D.velocity = new Vector2(playerRB2D.velocity.x, 0);
        //            playerRB2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //        }
        //    }
        //}
    }

    private void HandlePortal()
    {
        //this causes resetinig gravity and sprite visible into initial value, and item use won't work because of this
        //if(collisionHandler.getPortalValue() != 1 && isInPortal == false) //if player is not on the portal
        //{
        //    gravityScale = 9.8f;
        //    isTrigger = false;
        //    spriteVisible = true;
        //    isInPortal = false;
        //}

        if (collisionHandler.getPortalValue() == 1 && portalInput == 1)//entering portal
        {
            isInPortal = true;
            gravityScale = 0;
            isTrigger = true;
            spriteVisible = false;
        }

        if(isInPortal && portalInput == 2)//exiting portal when player is in portal
        {
            gravityScale = 9.8f;
            isTrigger = false;
            spriteVisible = true;
            isInPortal = false;
        }
    }

    private void UsingItem()
    {
        if (itemCode == 0)//value for double jump
        {
            if (IsGrounded())
            {
                usedJump = false;
            }
        }

        if (itemCode != -1 && useItemInput == 1)//dash item
        {
            //double jump item
            if(itemCode == 0 && !IsGrounded())
            {
                if (!usedJump)
                {
                    Debug.Log("double jump");
                    usedJump = true;
                    playerRB2D.velocity = new Vector2(playerRB2D.velocity.x, 0);
                    playerRB2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }

            //dash item
            if (itemCode == 1 && !usedDash)
            {
                Debug.Log("Use Dash");
                usedDash = true;
                gravityScale = 0;//set player gravity scale to  so that it looks like dash on air(no falling while dash)
                isMovable = false;
                playerRB2D.velocity = new Vector2(lastDirection * dashForce, 0);
                StartCoroutine("MoveDelay");
            }
        }

        else if (itemCode != -1 && useItemInput == 2)
        {
            //drop item
        }
    }

    private IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(0.2f);
        isMovable = true;
        gravityScale = 9.8f;
        yield return new WaitForSeconds(0.5f);
        usedDash = false;
    }

    private void representEmotion()
    {
        //show different emotion according to emotionInput value
    }

    public void SetItemCode(int itemNum)
    {
        itemCode = itemNum;
    }
    
    //for network below
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

    public void ChangeMovable()
    {
        isMovable = !isMovable;
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
