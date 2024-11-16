using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private float onPortal = 0;

    //if player is on the portal, return 1
    //if not, return 0
    public float getPortalValue()
    {
        return onPortal;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        //if collided object's tag is item
        //get item's properties(if it is double jump, dash, or weapon)
        //give information to player that the player got the item(ex. set hasItem = 1)
        //if(collision.gameObject.tag == "Item")
        //{
        //    Debug.Log("Collided");
        //    //collision.gameObject.GetComponent<ItemType>().GetSprite().enabled = false;
        //    //collision.gameObject.GetComponent<ItemType>().GetCollider().enabled = false;
        //    int itemNum = collision.gameObject.GetComponentInParent<ItemType>().GetItemCode();
        //    this.gameObject.GetComponent<PlayerController>().SetItemCode(itemNum);
        //}

        //if collided object's tag is enemy
        //player death
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Portal")
        {
            onPortal = 1;
            Debug.Log("Player is on Portal");
        }

        //if collided object's tag is item
        //get item's properties(if it is double jump, dash, or weapon)
        //give information to player that the player got the item(ex. set hasItem = 1)
        if (collision.gameObject.tag == "Item")
        {
            Debug.Log("Collided");
            collision.gameObject.GetComponent<ItemScript>().SetSprite();
            collision.gameObject.GetComponent<ItemScript>().SetCollider();
            collision.gameObject.GetComponent<ItemScript>().SetOffset();
            collision.gameObject.GetComponent<ItemScript>().SetPos(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);

            int itemNum = collision.gameObject.GetComponentInParent<ItemType>().GetItemCode();
            this.gameObject.GetComponent<PlayerController>().SetItemCode(itemNum);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Portal")
        {
            onPortal = 0;
        }
    }
}