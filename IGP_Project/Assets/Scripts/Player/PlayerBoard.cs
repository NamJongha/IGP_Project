using UnityEngine;
using Fusion;

public class PlayerBoard : NetworkBehaviour
{
    [Networked]
    public string PlayerColor { get; set; } // Synchronizing player's color

    void Start()
    {
        // ���⿡�� PlayerColor�� �ʱ�ȭ�� �� ����
        // ��: PlayerColor = "Red";
    }

    // �浹 �ν�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!HasStateAuthority)
            return;

        string layer = LayerMask.LayerToName(collision.gameObject.layer);

        if (layer == "Default" || layer == PlayerColor)
        {
            Debug.Log("���� �� �ֽ��ϴ�.");
        }
        else
        {
            Debug.Log("��ġ���� �ʴ� ���� �����Դϴ�. �Ʒ��� �������ϴ�.");
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}