using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handle Network Player

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }//accessing local network player(accessing the character the player controls)

   void Start()
    {

    }

    public override void Spawned()//called when starting the game(when spawned by network)
    {
        base.Spawned();
        if (Object.HasInputAuthority)//when the player who entered the network is yourself
        {
            Local = this;
            Debug.Log("Player Character Spawned");
        }
        else//when the player who entered the network is not you(when other players entered room)
        {
            //OtherPlayerCharacterSpawnmethod
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(player == Object.InputAuthority)//inputAuthority : the player who controls controller(keyboard...)
        {
            Runner.Despawn(Object);
        }

        Debug.Log("Player Disconnected");
        Runner.Despawn(Object);
        SceneManager.LoadScene("LobbyScene");
    }
}
