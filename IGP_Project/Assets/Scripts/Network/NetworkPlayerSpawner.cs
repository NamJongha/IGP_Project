using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

//Spawn players on network

public class NetworkPlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    PlayerInputHandler localPlayerInputHandler;

    public List<PlayerRef> playerList = new List<PlayerRef>();

    void Start()
    {
        
    }

    Vector3 GetRandomSpawnPoint()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");//can use if we make some spawn point with empty object

        if (spawnPoints.Length == 0)
        {
            return Vector3.zero;
        }
        else
        {
            return spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.position;
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //if (runner.Topology == Topologies.Shared)
        //{
        //    Debug.Log("connected and start prefab as local");

        //    runner.Spawn(playerPrefab, GetRandomSpawnPoint(), Quaternion.identity, runner.LocalPlayer);
        //}
        //else Debug.Log("connected to server"); // not running in the shared mode
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //if (runner.IsServer)
        //{
        //    Debug.Log("OnPlayerJoined");
        //    runner.Spawn(playerPrefab, GetRandomSpawnPoint(), Quaternion.identity, player);
        //}
        //else return;
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player has disconnected from server");
        if (!playerList.Contains(player))
        {
            playerList.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput Input)
    {
        //if the player on the screen is local player
        if(localPlayerInputHandler == null && NetworkPlayer.Local != null)
        {
            localPlayerInputHandler = NetworkPlayer.Local.GetComponent<PlayerInputHandler>();
        }

        //if the player on the screen is other player
        if (localPlayerInputHandler != null)
        {
            Input.Set(localPlayerInputHandler.GetNetworkInput());
        }
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

       if (runner.IsServer)
       {
            Debug.Log("Scene Load Done");

           //if the stage has spawnpoint, spawn player
           if (GameObject.FindWithTag("SpawnPoint"))
           {
               //if the player object is already spawned, don't spawn new one
               foreach (var player in runner.ActivePlayers)
               {
                   if (!runner.TryGetPlayerObject(player, out _))
                   {
                       runner.Spawn(playerPrefab, GetRandomSpawnPoint(), Quaternion.identity, player);
                   }
               }
           }
           else return;
       }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
