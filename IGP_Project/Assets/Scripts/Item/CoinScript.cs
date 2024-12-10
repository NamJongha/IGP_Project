using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CoinScript : NetworkBehaviour
{
    SpriteRenderer coinSprite;
    CircleCollider2D coinCollider;

    [Networked] public bool isAcquired { get; set; }

    private ChangeDetector change;

    void Start()
    {
        coinSprite = GetComponentInChildren<SpriteRenderer>();
        coinCollider = GetComponentInChildren<CircleCollider2D>();
    }

    public override void Spawned()
    {
        base.Spawned();

        change = GetChangeDetector(ChangeDetector.Source.SimulationState);

        coinSprite.enabled = true;
        coinCollider.enabled = true;

        isAcquired = false;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    public override void Render()
    {
        base.Render();

        foreach (var change in this.change.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(isAcquired):
                    // Detect the change and call the handler.
                    var aquiredReader = GetPropertyReader<bool>(nameof(isAcquired));
                    var (oldAcquire, newAcquired) = aquiredReader.Read(previousBuffer, currentBuffer);
                    OnAcquiredChanged(oldAcquire, newAcquired);
                    break;
            }
        }
    }

    public void OnAcquiredChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        //when isAcquired is true, make sprite and collider enabled false
        coinSprite.enabled = !newVal;
        coinCollider.enabled = !newVal;
    }
}
