using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class KeyScript : NetworkBehaviour
{
    private Vector3 keyPos;

    [Networked] private bool isFollowingPlayer { get; set; }
    [Networked] private bool isActivate { get; set; }

    private ChangeDetector changes;

    private SpriteRenderer keySprite;
    private CapsuleCollider2D keyCollider;

    private void Awake()
    {
        this.gameObject.SetActive(true);
        keySprite = GetComponentInChildren<SpriteRenderer>();
        keyCollider = GetComponentInChildren<CapsuleCollider2D>();
    }

    public override void Spawned()
    {
        base.Spawned();

        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        isFollowingPlayer = false;

        isActivate = true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        FollowPlayer();

        keySprite.enabled = isActivate;
        keyCollider.enabled = isActivate;
    }

    public override void Render()
    {
        base.Render();

        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(isFollowingPlayer):
                    var KeyReader = GetPropertyReader<bool>(nameof(isFollowingPlayer));
                    var (prevKey, curKey) = KeyReader.Read(previousBuffer, currentBuffer);
                    break;

                case nameof(isActivate):
                    var isActivateReader = GetPropertyReader<bool>(nameof(isActivate));
                    var (prevActivate, curActivate) = isActivateReader.Read(previousBuffer, currentBuffer);
                    break;

            }
        }
    }

    private void FollowPlayer()
    {
        if (isFollowingPlayer)
        {
            transform.position = new Vector3(keyPos.x, keyPos.y, keyPos.z);
        }
        else return;
    }


    public void SetFollow()
    {
        isFollowingPlayer = !isFollowingPlayer;
    }

    public void SetActivate()
    {
        isActivate = !isActivate;
    }

    public bool GetFollowState()
    {
        return isFollowingPlayer;
    }

    public void SetPos(Vector3 playerPos)
    {
        keyPos = playerPos;
    }

    private void OnIsFollowingPlayerChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        isFollowingPlayer = newVal;
    }

    private void OnIsActiveChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        isActivate = newVal;
    }
}
