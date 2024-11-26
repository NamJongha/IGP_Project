using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyController : NetworkBehaviour
{
    public float moveSpeed = 3f;

    [Networked] // ��Ʈ��ũ���� ����ȭ�� ����
    public bool isMovingRight { get; set; } = true; // �⺻���� true�� ����

    private SpriteRenderer sr;
    private Rigidbody2D rb2d;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.flipX = true;
    }

    public override void FixedUpdateNetwork()
    {
        Vector2 newVelocity = new Vector2(isMovingRight ? moveSpeed : -moveSpeed, rb2d.velocity.y);
        rb2d.velocity = newVelocity;
        sr.flipX = isMovingRight; // flipX ���� ������Ʈ
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���̾ Boundary�� ��쿡�� ������ ����
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Boundary")
        {
            isMovingRight = !isMovingRight;
        }
    }
}