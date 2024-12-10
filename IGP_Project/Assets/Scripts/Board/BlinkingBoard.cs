using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlinkingBoard : NetworkBehaviour
{
    private TilemapRenderer blinkBoardSprite;
    private TilemapCollider2D blinkBoardCollider;

    [Networked] private bool isHiding { get; set; }

    private ChangeDetector changes;

    private float waitTime = 5f;        //blink term

    private void Start()
    {
        blinkBoardCollider = GetComponent<TilemapCollider2D>();
        blinkBoardSprite = GetComponent<TilemapRenderer>();
    }

    public override void Spawned()
    {
        base.Spawned();

        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        isHiding = true;
        StartCoroutine(BoardHideOn());
    }

    public override void Render()
    {
        base.Render();

        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(isHiding):
                    var ButtonReader = GetPropertyReader<bool>(nameof(isHiding));
                    var (prevButton, curButton) = ButtonReader.Read(previousBuffer, currentBuffer);
                    OnBoardHide(prevButton, curButton);
                    break;
            }
        }
    }

    private void OnBoardHide(NetworkBool oldVal, NetworkBool newVal)
    {
        blinkBoardSprite.enabled = !newVal;
        blinkBoardCollider.enabled = !newVal;
    }

    IEnumerator BoardHideOn()
    {
        isHiding = true;
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(BoardHideOff());
    }

    IEnumerator BoardHideOff()
    {
        isHiding = false;
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(BoardHideOn());
    }
}
