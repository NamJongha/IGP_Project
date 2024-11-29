using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.UIElements;
using Fusion.Addons.Physics;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Colors")]
    [SerializeField] private Sprite BodyColor1;
    [SerializeField] private Sprite BodyColor2;
    [SerializeField] private Sprite BodyColor3;
    [SerializeField] private Sprite BodyColor4;

    private string lastBodyColor;
    [Networked] public string bodyColor { get; set; }

    [Header("Player Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 6.5f;
    public float dashForce = 5f;
    public float throwPow = 20f; //power of throwing item

    [Header("Player Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    //player input value
    float moveInput = 0;
    float jumpInput = 0;
    float portalInput = 0;
    float useItemInput = 0;
    float emotionInput = 0;

    [Networked] public bool isInPortal { get; set; }
    [Networked] public bool isPlayerDead {  get; set; }

    //variables for key
    private int itemCode = -1; //None: -1, double jump: 0, dash: 1, weapon: 2
    private GameObject curItem; //save acquired item's information
    private bool isMovable = true;//for dash, if it is false, player can't controll character
    private float lastDirection = 0;//for dash, save the last direction that player looked(moved)

    private GameObject curKey;

    //parameters below are for item delay(prevent infinite usage of item)
    private bool usedJump = false;
    private bool usedDash = false;

    //Current camera width Position
    float camwidthPosition;

    //Network variables for syncing players on network
    [Networked] private bool isTrigger { get; set; }
    [Networked] private float gravityScale { get; set; }
    [Networked] private bool spriteVisible { get; set; }
    [Networked] private Vector3 facePosition { get; set; }

    Rigidbody2D playerRB2D;
    PlayerCollisionHandler collisionHandler;
    BoxCollider2D playerCollider;
    CameraFollow cameraSize;

    SpriteRenderer[] playerSprite;
    SpriteRenderer playerBodySprite;
    SpriteRenderer playerFaceSprite;

    private ChangeDetector changes;

    private void Awake()
    {
        playerRB2D = GetComponent<Rigidbody2D>();
        collisionHandler = GetComponent<PlayerCollisionHandler>();
        playerCollider = GetComponentInChildren<BoxCollider2D>();
        cameraSize = GameObject.FindGameObjectWithTag("ViewPosition").GetComponent<CameraFollow>();

        playerSprite = GetComponentsInChildren<SpriteRenderer>();
        playerBodySprite = playerSprite[0];
        playerFaceSprite = playerSprite[1];

        curItem = null;
        curKey = null;
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

        facePosition = this.gameObject.transform.position;

        isInPortal = false;
        isPlayerDead = false;
        itemCode = -1;
        isMovable = true;
        lastDirection = 0;

        SetBodySprite();
    }

    //clinets get input data from host
    public override void FixedUpdateNetwork()
    {
        //Debug.Log(isPlayerDead);

        if(bodyColor != lastBodyColor)
        {
            lastBodyColor = bodyColor;
            SetBodySprite();
        }

        if(GetInput(out NetworkInputData data))
        {
            moveInput = data.direction.x;
            jumpInput = data.direction.y;
            portalInput = data.enterPortal;
            useItemInput = data.useItem;
        }

        if(moveInput != 0)
        {
            if(isMovable) lastDirection = moveInput;
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

                    SetFaceSprite();
                }
                UsingItem();

                HandlingKey();
                
                representEmotion();

                if (isPlayerDead)
                {
                    playerRB2D.velocity = Vector2.zero;
                    gravityScale = 0;
                }
            }
        }

        playerRB2D.gravityScale = gravityScale;
        playerCollider.isTrigger = isTrigger;
        playerBodySprite.enabled = spriteVisible;
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
                    var (prevItemSprite, curItemSprite) = spriteReader.Read(previousBuffer, currentBuffer);
                    OnSpriteVisibleChagned(prevItemSprite, curItemSprite);
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

                case nameof(OnInPortalChanged):
                    var inPortalReader = GetPropertyReader<bool>(nameof(OnInPortalChanged));
                    var (prevInPortal, curInPortal) = inPortalReader.Read(previousBuffer, currentBuffer);
                    OnInPortalChanged(prevInPortal, curInPortal);
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
        if (!collisionHandler.getOnBoundray())
        {
            playerRB2D.velocity = new Vector2(moveInput * moveSpeed, playerRB2D.velocity.y);
        }
        else
        {
            playerRB2D.velocity = Vector2.zero;
            MoveRestriction();
        }
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
                usedJump = false;//when double jumped, it turns into true until player land on ground
            }
        }

        if (itemCode != -1)
        {
            //make item follows the player position
            curItem.GetComponentInParent<ItemScript>().SetPos(new Vector3(this.gameObject.transform.position.x + ((float)lastDirection) * 2 / 3, this.gameObject.transform.position.y, this.gameObject.transform.position.z));

            if (isMovable) {
                if (useItemInput == 1)
                {
                    //double jump item
                    if (itemCode == 0 && !IsGrounded())
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

                //drop item
                else if (useItemInput == 2)
                {
                    Debug.Log("Drop Item");

                    //make item visible and collidable again and make rigidbody2D dynamic to throw it(addforce)
                    curItem.GetComponentInParent<ItemScript>().SetSprite();
                    curItem.GetComponentInParent<ItemScript>().SetCollider();
                    curItem.GetComponentInParent<ItemScript>().SetOffset();
                    curItem.GetComponentInParent<ItemScript>().SetRBDynamic();
                    curItem.GetComponentInParent<ItemScript>().SetFollow();

                    //throw the item(directtion is the way player last looked)
                    //curItem.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(lastDirection * throwPow, 3f), ForceMode2D.Impulse);
                    curItem.GetComponentInParent<Rigidbody2D>().gravityScale = 3f;
                    curItem.GetComponentInParent<Rigidbody2D>().velocity = Vector2.zero;
                    curItem.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(lastDirection * throwPow, 6.5f);

                    SetItemCode(-1);
                    SetCurItem(null);
                }
            }
        }
    }

    private void HandlingKey()
    {
        if(curKey != null)
        {
            curKey.GetComponentInParent<KeyScript>().SetPos(new Vector3(this.gameObject.transform.position.x + ((float)-lastDirection) * 2 / 3, this.gameObject.transform.position.y, this.gameObject.transform.position.z), lastDirection);
        }
    }

    //set movable when use dash
    private IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(0.2f);
        isMovable = true;
        gravityScale = 9.8f;
        yield return new WaitForSeconds(0.5f);
        usedDash = false;
    }

    private void MoveRestriction()
    {
        camwidthPosition = cameraSize.GetCameraWidthPosition();
        this.transform.position = new Vector3(Mathf.Clamp(transform.position.x, playerBodySprite.bounds.size.x * 0.4f - camwidthPosition, camwidthPosition - playerBodySprite.bounds.size.x * 0.6f),
                                                              transform.position.y);
    }
    private void representEmotion()
    {
        //show different emotion according to emotionInput value
    }

    public void SetFaceSprite()
    {
        facePosition = this.gameObject.transform.position + new Vector3(moveInput * 0.18f, 0, -0.1f);
        playerFaceSprite.transform.position = facePosition;
    }
    
    public void SetBodySprite()
    {
        switch (bodyColor)
        {
            case "Blue":
                playerBodySprite.sprite = BodyColor1;
                break;
            case "Green":
                playerBodySprite.sprite = BodyColor2;
                break;
            case "Pink":
                playerBodySprite.sprite = BodyColor3;
                break;
            case "Yellow":
                playerBodySprite.sprite = BodyColor4;
                break;
        }
    }

    public void SetBodyColor(string color)
    {
        bodyColor = color;
    }

    public void SetNotMovable()
    {
        isMovable = false;
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

    public void SetItemCode(int itemNum)
    {
        itemCode = itemNum;
    }

    public void SetCurItem(GameObject item)
    {
        curItem = item;
    }

    public void SetUseItem(float useItemPressed)
    {
        useItemInput = useItemPressed;
    }

    public void SetKey(GameObject key)
    {
        curKey = key;
    }

    public void SetEmotion(int emotionNum)
    {
        emotionInput = emotionNum;
    }
    public int GetItemCode()
    {
        return itemCode;
    }

    public GameObject GetKey()
    {
        return curKey;
    }

    public void ChangeMovable()
    {
        isMovable = !isMovable;
    }

    private void OnSpriteVisibleChagned(NetworkBool oldVal, NetworkBool newVal)
    {
        playerBodySprite.enabled = newVal;
        playerFaceSprite.enabled = newVal;
    }

    private void OnGravityScaleChanged(float oldVal, float newVal)
    {
        playerRB2D.gravityScale = newVal;
    }

    private void OnIsTriggerChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        playerCollider.isTrigger = newVal;
    }

    private void OnInPortalChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        isInPortal = newVal;
    }
}