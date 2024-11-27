using UnityEngine;
using Fusion;

public class SlidingBoard : NetworkBehaviour
{
    public float slideForce = 7f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NetworkObject playerNetworkObject = collision.gameObject.GetComponent<NetworkObject>();
            if (playerNetworkObject != null && playerNetworkObject.HasStateAuthority)
            {
                Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRigidbody != null)
                {
                    playerRigidbody.AddForce(transform.right * slideForce, ForceMode2D.Force);
                }
            }
        }
    }
}