using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyController : NetworkBehaviour
{
    public float moveSpeed = 3f;
    private bool isMovingRight = true;

    private SpriteRenderer sr;
    private Rigidbody2D rb2d;
    void Start()
    {
        rb2d  = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.flipX = true;
    }


    public override void FixedUpdateNetwork()
    {
        if (isMovingRight)
        {
            rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
            sr.flipX = true;
        }
        else
        {
            rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
            sr.flipX = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Boundary"))
            isMovingRight = !isMovingRight;
    }
}
