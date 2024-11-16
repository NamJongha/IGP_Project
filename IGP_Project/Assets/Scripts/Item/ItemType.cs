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
    [SerializeField] Item itemName;
    private int itemCode;

    private void Update()
    {
        switch (itemName)
        {
            case Item.DoubleJump:
                itemCode = 0;
                break;

            case Item.Dash:
                itemCode = 1;
                break;

            case Item.Weapon:
                itemCode = 2;
                break;
        }
    }

    public int GetItemCode()
    {
        return itemCode;
    }
}
