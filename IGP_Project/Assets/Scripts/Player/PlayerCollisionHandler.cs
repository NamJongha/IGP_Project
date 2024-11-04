using System.Collections;
using System.Collections.Generic;
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

    public void OnCollisionEnter2D()
    {
        //if collided object's tag is portal
        //change onPortal value to 1

        //if collided object's tag is item
        //get item's properties(if it is double jump, dash, or weapon)
        //give information to player that the player got the item(ex. set hasItem = 1)

        //if collided object's tag is enemy
        //player death
    }
}