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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Portal")
        {
            onPortal = 0;
        }
    }
}