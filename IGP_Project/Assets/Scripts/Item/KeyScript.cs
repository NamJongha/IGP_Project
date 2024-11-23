using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class KeyScript : NetworkBehaviour
{
    private Vector3 keyPos;

    [Networked] private bool isFollowingPlayer { get; set; }

    private ChangeDetector changes;

    private void Awake()
    {
        this.gameObject.SetActive(true);
    }

    public override void Spawned()
    {
        base.Spawned();

        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        isFollowingPlayer = false;
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
}
