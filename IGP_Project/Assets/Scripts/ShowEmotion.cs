using UnityEngine;
using System.Collections;
using Fusion;

public class ShowEmotion : NetworkBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite Good;
    public Sprite Angry;
    public Sprite Sorry;

    [Networked]
    public int activeEmotionIndex { get; set; } = -1; // -1 means the emotion is empty
    private bool isEmotionActive = false;

    private void Update()
    {
        if (!HasStateAuthority)
            return;

        if (!isEmotionActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeSprite(0); // Good
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeSprite(1); // Angry
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeSprite(2); // Sorry
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
}