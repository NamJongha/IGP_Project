using UnityEngine;
using Fusion;

public class SlidingBoard : NetworkBehaviour
{
    private float slideForce = 50f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(new Vector2(player.lastDirection * slideForce, player.transform.position.y), ForceMode2D.Force);
            }
        }
    }
}