using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ItemScript : NetworkBehaviour
{
    //Transform itemTransform;
    SpriteRenderer itemSprite;
    CircleCollider2D itemCollider;
    BoxCollider2D itemGroundOffset;
    Rigidbody2D itemRB;

    private Vector3 itemPos;

    [Networked] public bool offsetCollider { get; set; }
    [Networked] public bool _itemCollider { get; set; }
    [Networked] public bool isSpriteVisible { get; set; }
    [Networked] public bool _isFollowingPlayer { get; set; }
    [Networked] public bool rigidbodyDynamic { get; set; }

    private ChangeDetector changes;

    //when player get item, item will follow the player in invisable state
    //private bool isFollowingPlayer = false;

    private void Awake()
    {
        itemSprite = GetComponentInChildren<SpriteRenderer>();
        itemCollider = GetComponentInChildren<CircleCollider2D>();
        itemGroundOffset = GetComponentInChildren<BoxCollider2D>();
        itemRB = GetComponent<Rigidbody2D>();

        itemPos = transform.position;
    }

    public override void Spawned()
    {
        base.Spawned();
        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        itemSprite.enabled = true;
        itemCollider.enabled = true;
        itemGroundOffset.enabled = true;
        _isFollowingPlayer = false;
        rigidbodyDynamic = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (rigidbodyDynamic)
        {
            itemRB.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            itemRB.bodyType = RigidbodyType2D.Kinematic;
        }

        FollowPlayer();
    }

    public override void Render()
    {
        base.Render();

        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(isSpriteVisible):
                    var spriteReader = GetPropertyReader<bool>(nameof(isSpriteVisible));
                    var (prevSprite, curSprite) = spriteReader.Read(previousBuffer, currentBuffer);
                    OnIsSpriteVisibleChanged(prevSprite, curSprite);
                    break;

                case nameof(offsetCollider):
                    var triggerReader = GetPropertyReader<bool>(nameof(offsetCollider));
                    var (prevEnabled, curEnabled) = triggerReader.Read(previousBuffer, currentBuffer);
                    OnOffsetColliderChagned(prevEnabled, curEnabled);
                    break;

                case nameof(_itemCollider):
                    var gravityReader = GetPropertyReader<bool>(nameof(_itemCollider));
                    var (prevItemCollider, curItemCollider) = gravityReader.Read(previousBuffer, currentBuffer);
                    On_ItemColliderChanged(prevItemCollider, curItemCollider);
                    break;

                case nameof(_isFollowingPlayer):
                    var followingReader = GetPropertyReader<bool>(nameof(_isFollowingPlayer));
                    var (prevFollowingState, curFollowingState) = followingReader.Read(previousBuffer, currentBuffer);
                    On_isFollowingPlayerChanged(prevFollowingState, curFollowingState);
                    break;

                case nameof(rigidbodyDynamic):
                    var rbDynamicReader = GetPropertyReader<bool>(nameof(rigidbodyDynamic));
                    var (prevDynamicState, curDynamicState) = rbDynamicReader.Read(previousBuffer, currentBuffer);
                    OnrigidbodyDynamicChanged(prevDynamicState, curDynamicState);
                    break;
            }
        }
    }

    //give player's position as parameter
    private void FollowPlayer()
    {
        if (_isFollowingPlayer)
        {
            transform.position = new Vector3(itemPos.x, itemPos.y, itemPos.z);
        }
        else return;
    }

    public void SetPos(Vector3 playerPos)
    {
        itemPos = playerPos;
    }

    public void SetSprite()
    {
        isSpriteVisible = !isSpriteVisible;
    }

    public void SetCollider()
    {
        _itemCollider = !_itemCollider;
    }

    public void SetOffset()
    {
        offsetCollider = !offsetCollider;
    }

    public void SetFollow()
    {
        _isFollowingPlayer = !_isFollowingPlayer;
    }

    public void SetRBDynamic()
    {
        rigidbodyDynamic = !rigidbodyDynamic;
    }

    private void OnIsSpriteVisibleChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        itemSprite.enabled = newVal;
    }
    
    private void OnOffsetColliderChagned(NetworkBool oldVal, NetworkBool newVal)
    {
        itemGroundOffset.enabled = newVal;
    }

    private void On_ItemColliderChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        itemCollider.enabled = newVal;
    }

    private void On_isFollowingPlayerChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        _isFollowingPlayer = newVal;
    }

    private void OnrigidbodyDynamicChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        rigidbodyDynamic = newVal;
    }
}