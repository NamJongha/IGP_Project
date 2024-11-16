using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    Transform itemTransform;
    SpriteRenderer itemSprite;
    CircleCollider2D itemCollider;
    BoxCollider2D itemGroundOffset;

    private float x, y, z;

    //when player get item, item will follow the player in invisable state
    private bool isFollowingPlayer = false;

    private void Awake()
    {
        itemTransform = GetComponent<Transform>();
        itemSprite = GetComponentInChildren<SpriteRenderer>();
        itemCollider = GetComponentInChildren<CircleCollider2D>();
        itemGroundOffset = GetComponentInChildren<BoxCollider2D>();
    }

    private void Update()
    {
        FollowPlayer();
    }

    //give player's position as parameter
    private void FollowPlayer()
    {
        if (isFollowingPlayer)
        {
            itemTransform.position = new Vector3(x, y, z);
        }
        else return;
    }

    public void SetPos(float playerX, float playerY, float playerZ)
    {
        x = playerX;
        y = playerY;
        z = playerZ;
    }

    public void SetSprite()
    {
        itemSprite.enabled = !itemSprite.enabled;
    }

    public void SetCollider()
    {
        itemCollider.enabled = !itemCollider.enabled;
    }

    public void SetOffset()
    {
        itemGroundOffset.enabled = !itemGroundOffset.enabled;
    }

    public void SetFollow()
    {
        isFollowingPlayer = !isFollowingPlayer;
    }
}
