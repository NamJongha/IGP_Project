using UnityEngine;
using Fusion;

public class PlayerBoard : NetworkBehaviour
{
    [Networked]
    public string PlayerColor { get; set; } // Synchronizing player's color

    void Start()
    {
        PlayerColor = GetComponentInParent<PlayerController>().bodyColor;
    }

    // 충돌 인식
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Untagged") && !collision.gameObject.CompareTag("Player"))
        {
            switch (PlayerColor)
            {
                case "Blue":
                    if (!collision.gameObject.CompareTag(PlayerColor))
                    {
                        Debug.Log(PlayerColor);
                        Physics2D.IgnoreCollision(collision.collider, GetComponentInChildren<Collider2D>());
                        Debug.Log(PlayerColor + " is not same");
                    }
                    break;
                case "Green":
                    if (!collision.gameObject.CompareTag(PlayerColor))
                    {
                        Debug.Log(PlayerColor);
                        Physics2D.IgnoreCollision(collision.collider, GetComponentInChildren<Collider2D>());
                        Debug.Log(PlayerColor + " is not same");
                    }
                    break;
                case "Pink":
                    if (!collision.gameObject.CompareTag(PlayerColor))
                    {
                        Debug.Log(PlayerColor);
                        Physics2D.IgnoreCollision(collision.collider, GetComponentInChildren<Collider2D>());
                        Debug.Log(PlayerColor + " is not same");
                    }
                    break;
                case "Yellow":
                    if (!collision.gameObject.CompareTag(PlayerColor))
                    {
                        Debug.Log(PlayerColor);
                        Physics2D.IgnoreCollision(collision.collider, GetComponentInChildren<Collider2D>());
                        Debug.Log(PlayerColor + " is not same");
                    }
                    break;
            }
        }
    }
}