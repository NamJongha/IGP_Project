using UnityEngine;
using Fusion;

public class PlayerBoard : NetworkBehaviour
{
    [Networked]
    public string PlayerColor { get; set; } // Synchronizing player's color

    void Start()
    {
        // 여기에서 PlayerColor를 초기화할 수 있음
        // 예: PlayerColor = "Red";
    }

    // 충돌 인식
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!HasStateAuthority)
            return;

        string layer = LayerMask.LayerToName(collision.gameObject.layer);

        if (layer == "Default" || layer == PlayerColor)
        {
            Debug.Log("밟을 수 있습니다.");
        }
        else
        {
            Debug.Log("일치하지 않는 색의 발판입니다. 아래로 떨어집니다.");
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}