using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyController : NetworkBehaviour
{
    public float moveSpeed = 3f;

    [Networked] // 네트워크에서 동기화할 변수
    public bool isMovingRight { get; set; } = true; // 기본값을 true로 설정

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
        sr.flipX = isMovingRight; // flipX 상태 업데이트
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 레이어가 Boundary인 경우에만 방향을 변경
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Boundary")
        {
            isMovingRight = !isMovingRight;
        }
    }
}