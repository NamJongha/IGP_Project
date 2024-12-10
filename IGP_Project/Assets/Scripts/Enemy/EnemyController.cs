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

    private ChangeDetector changes;

    private NetworkGameManager gameManager;

    private Animator enemyAnimator;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.flipX = true;

        enemyAnimator = GetComponentInChildren<Animator>();

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<NetworkGameManager>();
    }

    public override void Spawned()
    {
        base.Spawned();

        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        sr.flipX = true;

        enemyAnimator.SetBool("isPlayerDead", false);
    }

    public override void FixedUpdateNetwork()
    {

        //Debug.Log(gameManager.GetPlayerDead());
        if (HasStateAuthority)
        {
            enemyAnimator.SetBool("isPlayerDead", false);

            if (gameManager.GetPlayerDead())
            {
                enemyAnimator.SetBool("isPlayerDead", true);
                rb2d.velocity = Vector2.zero;
                return;
            }
        }

        Vector2 newVelocity = new Vector2(isMovingRight ? moveSpeed : -moveSpeed, rb2d.velocity.y);
        rb2d.velocity = newVelocity;
    }

    public override void Render()
    {
        base.Render();

        foreach(var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(isMovingRight):
                    var isMovingRightReader = GetPropertyReader<bool>(nameof(isMovingRight));
                    var (prevMoving, curMoving) = isMovingRightReader.Read(previousBuffer, currentBuffer);
                    OnIsMovingRightChagned(prevMoving, curMoving);
                    break;

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 레이어가 Boundary인 경우에만 방향을 변경
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Boundary")
        {
            isMovingRight = !isMovingRight;
        }
    }

    private void OnIsMovingRightChagned(NetworkBool oldVal, NetworkBool newVal)
    {
        sr.flipX = newVal;
    }
}