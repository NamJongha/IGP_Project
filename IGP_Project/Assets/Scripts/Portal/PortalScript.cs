using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : NetworkBehaviour
{
    [Header("Portal State Sprite")]
    [SerializeField] private SpriteRenderer midSprite;
    [SerializeField] private SpriteRenderer topSprite;
    public Sprite lockedSprite_mid;
    public Sprite openedSprite_mid;
    public Sprite lockedSprite_top;
    public Sprite openedSprite_top;

    [Header("Next Scene Name")]
    [SerializeField] public string sceneName;

    [Networked] public bool isPortalLocked { get; set; }

    private ChangeDetector changes;

    private void Awake()
    {
        if (GameObject.FindWithTag("Key"))
        {
            isPortalLocked = true;
        }
        else
        {
            isPortalLocked = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if(isPortalLocked)
        {
            midSprite.sprite = lockedSprite_mid;
            topSprite.sprite = lockedSprite_top;
        }
        else
        {
            midSprite.sprite = openedSprite_mid;
            topSprite.sprite = openedSprite_top;
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if (GameObject.FindWithTag("Key"))
        {
            isPortalLocked = true;
        }
        else
        {
            isPortalLocked = false;
        }
    }

    public override void Render()
    {
        base.Render();

        foreach(var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch(change)
            {
                case nameof(isPortalLocked):
                    var lockedReader = GetPropertyReader<bool>(nameof(isPortalLocked));
                    var (prevLocked, curLocked) = lockedReader.Read(previousBuffer, currentBuffer);
                    break;
            }
        }
    }

    public void MoveToNextStage()
    {
        
    }

    public void SetLocked()
    {
        isPortalLocked = !isPortalLocked;
    }

    private void OnLockedChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        isPortalLocked = newVal;
    }
}
