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
        //if collided object's tag is enemy
        //player death
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Set player's flag of portal. if player is on the portal, player's portal value becomes 1
        if (collision.gameObject.tag == "Portal")
        {
            //if portal is not locked, player can interact with portal
            if (collision.gameObject.GetComponentInParent<PortalScript>().isPortalLocked == false)
            {
                onPortal = 1;
            }

            //if portal is locked
            else
            {
                //if player has key
                if (this.gameObject.GetComponent<PlayerController>().GetKey() != null)
                {
                    //unlock the portal with key. Key will be disappeared after using it.
                    collision.gameObject.GetComponentInParent<PortalScript>().SetLocked();
                    this.gameObject.GetComponent<PlayerController>().GetKey().transform.parent.gameObject.GetComponent<KeyScript>().SetActiveState();
                    this.gameObject.GetComponent<PlayerController>().SetKey(null);
                }

                else return;
            }
        }

        //if collided object's tag is item
        //get item's properties(if it is double jump, dash, or weapon)
        //give information to player that the player got the item(ex. set hasItem = 1)
        if (collision.gameObject.tag == "Item")
        {
            Debug.Log("Collided");
            if (this.gameObject.GetComponent<PlayerController>().GetItemCode() == -1)
            {
                //make item invisible and not collidable
                collision.gameObject.GetComponentInParent<ItemScript>().SetSprite();//if want to debug that if acquired item is following the player, make this line annotation
                collision.gameObject.GetComponentInParent<ItemScript>().SetCollider();
                collision.gameObject.GetComponentInParent<ItemScript>().SetOffset();
                collision.gameObject.GetComponentInParent<ItemScript>().SetRBDynamic();
                
                //set itemCode
                int itemNum = collision.gameObject.GetComponentInParent<ItemType>().GetItemCode();

                //make invisible item follows player
                this.gameObject.GetComponent<PlayerController>().SetItemCode(itemNum);
                this.gameObject.GetComponent<PlayerController>().SetCurItem(collision.gameObject);
                collision.gameObject.GetComponentInParent<ItemScript>().SetFollow();
            }
            else
            {
                Debug.Log("Already have Item");
                return;
            }
        }

        //if player collided with key, and the key is not following player
        if(collision.gameObject.tag == "Key" && collision.gameObject.GetComponentInParent<KeyScript>().GetFollowState() == false)
        {
            //make key follows the player
            this.gameObject.GetComponent<PlayerController>().SetKey(collision.gameObject);
            collision.gameObject.GetComponentInParent<KeyScript>().SetFollow();
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