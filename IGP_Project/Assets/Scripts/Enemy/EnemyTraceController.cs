using Fusion;
using UnityEngine;

public class EnemyTraceController : NetworkBehaviour
{
    [Networked] public bool isMovingRight { get; set; } = true;

    private SpriteRenderer enemySprite;
    private Rigidbody2D enemyRB2D;
    private Animator enemyAnimator;
    private NetworkGameManager gameManager;
    public GameObject[] players;
    public GameObject targetPlayer;

    private ChangeDetector changes;

    public float moveSpeed = 0.5f;
    public float chaseRange = 1f;
    public bool isChasing = false;
    private float closestDistance = Mathf.Infinity;


    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<NetworkGameManager>();
        players = gameManager.GetComponent<NetworkPortalHandler>().playerList;
        enemyRB2D = GetComponent<Rigidbody2D>();
        enemySprite = GetComponentInChildren<SpriteRenderer>();
        enemyAnimator = GetComponentInChildren<Animator>();
        enemySprite.flipX = false;
    }
    public override void Spawned()
    {
        base.Spawned();
        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        enemyAnimator.SetBool("isPlayerDead", false);
    }
    // Update is called once per frame
    public void Update()
    {

        
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasStateAuthority)
        {
            if (gameManager.GetPlayerDead())
            {
                enemyAnimator.SetBool("isPlayerDead", true);
                enemyRB2D.velocity = Vector2.zero;
                return;
            }

            TargetPlayer();

            if (targetPlayer != null)
            {

                float distanceToTarget = Vector3.Distance(transform.position, targetPlayer.transform.position);
                if (distanceToTarget < chaseRange)
                {
                    isChasing = true;
                    CheckRight();
                    ChaseTarget();
                }
                else
                {
                    isChasing = false;
                }
            }
        }
    }

    void TargetPlayer()
    {
        players = gameManager.GetComponent<NetworkPortalHandler>().playerList;
        targetPlayer = null;
        closestDistance = Mathf.Infinity;

        Vector3 currentPosition = transform.position;

        foreach (GameObject player in players)
        {
            if (player != null)
            {
                float distance = Vector3.Distance(currentPosition, player.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetPlayer = player;
                }
            }
        }
    }
    private void ChaseTarget()
    {
        Vector3 targetPosition = targetPlayer.transform.position;

        Vector3 currentPosition = transform.position;

        float step = moveSpeed * Time.deltaTime;
        Vector3 newPosition = new Vector3(targetPosition.x, currentPosition.y, currentPosition.z);

        transform.position = Vector3.MoveTowards(currentPosition, newPosition, step);
    }
    public override void Render()
    {
        base.Render();

        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary"))
        {
            isChasing = false;
        }
    }

    void CheckRight()
    {
        if (targetPlayer.transform.position.x < transform.position.x)
        {
            isMovingRight = false;
        }
        else
        {
            isMovingRight = true;
        }
    }
    private void OnIsMovingRightChagned(NetworkBool oldVal, NetworkBool newVal)
    {
        enemySprite.flipX = newVal;
    }
}
