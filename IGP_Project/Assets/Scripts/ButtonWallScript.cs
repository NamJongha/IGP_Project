using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ButtonWallScript : NetworkBehaviour
{
    private SpriteRenderer buttonWallSprite;
    private BoxCollider2D buttonWallCollider;

    [Header("Connected Button")]
    [SerializeField] private GameObject buttonInStage;

    [Networked] public bool isConnectedButtonPressed { get; set; }

    private ChangeDetector changes;

    void Start()
    {
        buttonWallSprite = GetComponentInChildren<SpriteRenderer>();
        buttonWallCollider = GetComponentInChildren<BoxCollider2D>();
    }

    public override void Spawned()
    {
        base.Spawned();

        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        isConnectedButtonPressed = false;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (buttonInStage.GetComponent<ButtonScript>().GetIsPressed())
        {
            isConnectedButtonPressed = true;
        }
    }

    public override void Render()
    {
        base.Render();

        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(isConnectedButtonPressed):
                    var ButtonReader = GetPropertyReader<bool>(nameof(isConnectedButtonPressed));
                    var (prevButton, curButton) = ButtonReader.Read(previousBuffer, currentBuffer);
                    OnButtonPressedChanged(prevButton, curButton);
                    break;
            }
        }
    }

    private void OnButtonPressedChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        buttonWallSprite.enabled = !newVal;
        buttonWallCollider.enabled = !newVal;
    }
}
