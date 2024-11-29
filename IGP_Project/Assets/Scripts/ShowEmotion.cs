using UnityEngine;
using System.Collections;
using Fusion;

public class ShowEmotion : NetworkBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite Good;
    public Sprite Angry;
    public Sprite Sorry;

    private float emotionInput = 0;

    [Networked, OnChangedRender(nameof(OnEmotionIndexChanged))]
    public int activeEmotionIndex { get; set; } = -1; // -1 means the emotion is empty
    private bool isEmotionActive = false;

    private ChangeDetector change;

    public override void Spawned()
    {
        base.Spawned();

        change = GetChangeDetector(ChangeDetector.Source.SimulationState);

        activeEmotionIndex = -1;

    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput(out NetworkInputData data))
        {
            emotionInput = data.emotion;
        }

        if (HasStateAuthority)
        {
            representEmotion();
        }
    }

    private void representEmotion()
    {
        if (!isEmotionActive)
        {
            if (emotionInput == 1)
            {
                ChangeSprite(0);
            }

            if (emotionInput == 2)
            {
                ChangeSprite(1);
            }

            if (emotionInput == 3)
            {
                ChangeSprite(2);
            }
        }


        UpdateSprite();
    }

    private void ChangeSprite(int emotionIndex)
    {
        activeEmotionIndex = emotionIndex;
        isEmotionActive = true;
        StartCoroutine(ResetSpriteAfterDelay(3f));
    }

    private IEnumerator ResetSpriteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        activeEmotionIndex = -1;
        isEmotionActive = false;
    }

    private void UpdateSprite()
    {
        switch (activeEmotionIndex)
        {
            case 0:
                spriteRenderer.sprite = Good;
                break;
            case 1:
                spriteRenderer.sprite = Angry;
                break;
            case 2:
                spriteRenderer.sprite = Sorry;
                break;
            default:
                spriteRenderer.sprite = null;
                break;
        }
    }

    public void SetEmotion(float emotionNum)
    {
        emotionInput = emotionNum;
    }

    public void OnEmotionIndexChanged()
    {
        UpdateSprite();
    }
}