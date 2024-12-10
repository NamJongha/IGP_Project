using Fusion;
using UnityEngine;

public enum PlayerColor
{
    Blue = 0,
    Green = 1,
    Pink = 2,
    Yellow = 3
};

public class EnemyJump : NetworkBehaviour
{
    [Networked] private bool isflip { get; set; } = false;
    [Networked] private int targetIndex { get; set; } = 0;

    [Header("Enemy Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    Rigidbody2D enemyRG2D;
    BoxCollider2D enemyCollider;
    SpriteRenderer enemySprite;
    GameObject targetPlayer;
    private NetworkGameManager gameManager;
    public GameObject[] players;

    private ChangeDetector changes;

    public PlayerColor checkColor;
    private string targetColor;
    private int numberOfPlayers;
    private int lastNumOfPlayers;
    private float jumpStartY;
    private float jumpForce = 7f;
    private float maxJumpHeight = 1.8f;
    public override void Spawned()
    {
        base.Spawned();
        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        checkColor = PlayerColor.Blue;
        numberOfPlayers = 0;
        lastNumOfPlayers = numberOfPlayers;
        jumpStartY = transform.position.y;
        enemySprite.flipX = false;
    }
    public override void Render()
    {
        base.Render();
        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(isflip):
                    var isFilpReader = GetPropertyReader<bool>(nameof(isflip));
                    var (prevSprite, curSprite) = isFilpReader.Read(previousBuffer, currentBuffer);
                    OnFlipChange(prevSprite, curSprite);
                    break;
                case nameof(targetIndex):
                    var targetIndexReader = GetPropertyReader<int>(nameof(targetIndex));
                    var (prevTargetIndex, curTargetIndex) = targetIndexReader.Read(previousBuffer, currentBuffer);
                    TargetIndexChange(prevTargetIndex, curTargetIndex);
                    break;
            }
        }
    }

    
    void Awake()
    {
        enemyRG2D = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponentInChildren<BoxCollider2D>();
        enemySprite = GetComponentInChildren<SpriteRenderer>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<NetworkGameManager>();
        players = gameManager.GetComponent<NetworkPortalHandler>().playerList;
    }

    void LateUpdate()
    {
        numberOfPlayers = players.Length;
        if (lastNumOfPlayers != numberOfPlayers)
        {
            lastNumOfPlayers = numberOfPlayers;
            checkColor = (PlayerColor)Random.Range(0, lastNumOfPlayers);
        }
        CheckTargetPlayerColor();
        Debug.Log("Number of objects with 'Player' tag: " + lastNumOfPlayers);
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasStateAuthority)
        {
            enemyRG2D.rotation = 0;
            if (targetPlayer != null)
            {
                if (IsGrounded())
                    isflip = false;

                Jumping();
            }
            if (gameManager.GetPlayerDead())
            {
                enemyRG2D.velocity = Vector2.zero;
                isflip = false;
                return;
            }
        }
        Debug.Log("PlayerColor: " + checkColor + ",  PlayerNumber: " + targetIndex +",  player jump: " + IsGrounded());
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
    }

    private void CheckTargetPlayerColor()
    {
        players = gameManager.GetComponent<NetworkPortalHandler>().playerList;
        numberOfPlayers = players.Length;
        switch (checkColor)
        {
            case PlayerColor.Blue:
                targetIndex = 0;
                targetColor = "Blue";
                break;
            case PlayerColor.Green:
                targetIndex = 1;
                targetColor = "Green";
                break;
            case PlayerColor.Pink:
                targetIndex = 2;
                targetColor = "Pink";
                break;
            case PlayerColor.Yellow:
                targetIndex = 3;
                targetColor = "Yellow";
                break;
            default:
                targetIndex = 0;
                targetColor = "Blue";
                break;
        }
        if (targetIndex < numberOfPlayers)
        {
            targetPlayer = players[targetIndex];
            Debug.Log("target color: " + checkColor + ", Player Color: " + targetPlayer.GetComponent<PlayerController>().bodyColor);
        }
        else
        {
            Debug.LogWarning("No player found at index " + targetIndex + ". Current player count: " + numberOfPlayers);
            targetPlayer = players[0];
        }
    }
    private void Jumping()
    {
        if (targetPlayer.GetComponent<PlayerController>().bodyColor == targetColor)
        {
            if (targetPlayer.GetComponent<PlayerController>().jumpInput == 1 && IsGrounded())
            {
                enemyRG2D.velocity = new Vector2(enemyRG2D.velocity.x, 0);
                enemyRG2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            if (!IsGrounded() && transform.position.y >= jumpStartY + maxJumpHeight)
            {
                ChangeSpriteAngle();
            }
        }
    }

    private void ChangeSpriteAngle()
    {
        isflip = true;
    }

    private void OnFlipChange(NetworkBool oldVal, NetworkBool newVal)
    {
        enemySprite.flipX = newVal;
    }

    private void TargetIndexChange(int oldVal, int newVal)
    {
        targetIndex = newVal;
    }
}
