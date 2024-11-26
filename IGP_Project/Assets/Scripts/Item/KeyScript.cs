using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class KeyScript : NetworkBehaviour
{
    private Vector3 keyPos;
    private Vector3 keyDir;
    private float keyLastDirection = 1;

    [Networked] private bool isFollowingPlayer { get; set; }
    [Networked] public bool isActivate { get; set; }
    [Networked] private Vector3 NetworkedScale { get; set; }

    private ChangeDetector changes;

    private SpriteRenderer keySprite;
    private CapsuleCollider2D keyCollider;

    private void Awake()
    {
        keySprite = GetComponentInChildren<SpriteRenderer>();
        keyCollider = GetComponentInChildren<CapsuleCollider2D>();

        keyDir = transform.localScale;
    }

    public override void Spawned()
    {
        base.Spawned();

        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        isFollowingPlayer = false;

        isActivate = true;
        keySprite.enabled = true;
        keyCollider.enabled = true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        FollowPlayer();
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
                    OnIsFollowingPlayerChanged(prevKey, curKey);
                    break;

                case nameof(isActivate):
                    var isActivateReader = GetPropertyReader<bool>(nameof(isActivate));
                    var (prevActivate, curActivate) = isActivateReader.Read(previousBuffer, currentBuffer);
                    OnIsActiveChanged(prevActivate, curActivate);
                    break;

            }

            transform.localScale = NetworkedScale;
        }
    }

    private void FollowPlayer()
    {
        if (isFollowingPlayer)
        {
            transform.position = new Vector3(keyPos.x, keyPos.y, keyPos.z);
            NetworkedScale = new Vector3(keyLastDirection * keyDir.x, keyDir.y, keyDir.z);
            transform.localScale = NetworkedScale;
        }
        else return;
    }


    public void SetFollow()
    {
        isFollowingPlayer = !isFollowingPlayer;
    }

    public void SetActiveState()
    {
        isActivate = !isActivate;
    }

    public bool GetFollowState()
    {
        return isFollowingPlayer;
    }

    public void SetPos(Vector3 playerPos, float lastDirection)
    {
        keyPos = playerPos;
        keyLastDirection = lastDirection;
    }

    private void OnIsFollowingPlayerChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        isFollowingPlayer = newVal;
    }

    private void OnIsActiveChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        keySprite.enabled = newVal;
        keyCollider.enabled = newVal;
    }
}
