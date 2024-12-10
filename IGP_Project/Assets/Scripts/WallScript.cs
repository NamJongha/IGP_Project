using UnityEngine;
using Fusion;
using System.Linq;

public class WallScript : NetworkBehaviour
{
    private SpriteRenderer wallSprite;
    private BoxCollider2D wallCollider;

    private GameObject[] coinsInStage;

    private bool curCoins = false;
    [Networked] private bool gotAllCoins { get; set; }

    private ChangeDetector changes;

    void Start()
    {
        wallSprite = GetComponent<SpriteRenderer>();
        wallCollider = GetComponent<BoxCollider2D>();

        curCoins = false;
    }

    public override void Spawned()
    {
        base.Spawned();

        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        gotAllCoins = false;

        coinsInStage = FindCoinObjects();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Runner.IsServer)
        {
            CheckCoinAcquired();

        }

        Debug.Log(gotAllCoins);
    }

    public override void Render()
    {
        base.Render();

        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(gotAllCoins):
                    var CoinReader = GetPropertyReader<bool>(nameof(gotAllCoins));
                    var (prevCoin, curCoin) = CoinReader.Read(previousBuffer, currentBuffer);
                    OnGotAllCoinChanged(prevCoin, curCoin);
                    break;
            }
        }
    }

   // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   // {
   //
   //
   //     coinsInStage = FindCoinObjects();
   // }

    private GameObject[] FindCoinObjects()
    {
        //add to array only the coin object doesn't have parent (don't add child object)
        return GameObject.FindGameObjectsWithTag("Coin")
                         .Where(player => player.transform.parent == null)
                         .ToArray();
    }

    private void CheckCoinAcquired()
    {
        curCoins = false;

        if (coinsInStage == null || coinsInStage.Length != FindCoinObjects().Length)
        {
            coinsInStage = FindCoinObjects();
        }

        foreach (var coin in coinsInStage)
        {
            if(coin.GetComponent<CoinScript>().isAcquired == true)
            {
                curCoins = true;
            }
            else
            {
                curCoins = false;
                break;
            }
        }

        if (curCoins)
        {
            SetGotAllCoins();
        }
    }

    private void SetGotAllCoins()
    {
        gotAllCoins = true;
    }

    private void OnGotAllCoinChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        wallSprite.enabled = !newVal;
        wallCollider.enabled = !newVal;
    }
}
