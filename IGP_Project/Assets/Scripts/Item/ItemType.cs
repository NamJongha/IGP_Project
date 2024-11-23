using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item
{
    DoubleJump,
    Dash,
    Weapon
}

public class ItemType : MonoBehaviour
{
    [Header("Item Type")]
    [SerializeField] Item itemName;

    //If the type is given, item's sprite will be automatically changed
    [Header("Item Sprite")]
    [SerializeField] Sprite doubleJumpSprite;
    [SerializeField] Sprite dashSprite;
    [SerializeField] Sprite weaponSprite;

    private int itemCode;
    private SpriteRenderer itemSprite;

    private void Awake()
    {
        itemSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        switch (itemName)
        {
            case Item.DoubleJump:
                itemCode = 0;
                itemSprite.sprite = doubleJumpSprite;
                break;

            case Item.Dash:
                itemCode = 1;
                itemSprite.sprite = dashSprite;
                break;

            case Item.Weapon:
                itemCode = 2;
                itemSprite.sprite = weaponSprite;
                break;
        }
    }

    public int GetItemCode()
    {
        return itemCode;
    }
}
