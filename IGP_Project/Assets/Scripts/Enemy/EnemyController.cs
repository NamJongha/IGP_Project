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
        if (collision.CompareTag("Boundary"))
        {
            // 상태 권한이 있는 경우에만 이동 방향 변경
            if (HasStateAuthority)
            {
                // 방향을 반전시키고 이 변경을 네트워크에 동기화
                isMovingRight = !isMovingRight;
            }
        }
    }
}