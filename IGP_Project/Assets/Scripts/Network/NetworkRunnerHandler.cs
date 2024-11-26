using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;
using TMPro;
using NanoSockets;
using UnityEngine.UI;

public class NetworkRunnerHandler : MonoBehaviour
{
    //other components
    NetworkRunner networkRunner;

    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private Button enterRoomButton;
    [SerializeField] private Canvas LobbyUI;
    private String roomCode;

    private void Awake()
    {
        networkRunner = GetComponent<NetworkRunner>();

        if (enterRoomButton != null)
        {
            enterRoomButton.onClick.AddListener(StartWithRoomCode);
        }
    }

    void StartWithRoomCode()
    {
        roomCode = roomName.text;

        if (roomCode != null)
        {
            var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), null);
            LobbyUI.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Please enter the room code");
        }
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized) 
    {
        var sceneObjectProvider = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if(sceneObjectProvider == null)
        {
            //handle network objects already exist in the scene
            sceneObjectProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        //client provides input to host
        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = SceneRef.FromIndex(1),
            SessionName = roomCode,
            OnGameStarted = initialized,
            SceneManager = sceneObjectProvider
        });
    }

    private void OnGameStarted(NetworkRunnerHandler runner)
    {
        //Debug.Log("GameStart");
        //SceneManager.LoadScene("Monster_1");
    }
}


//For Debug below
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Fusion;
//using Fusion.Sockets;
//using UnityEngine.SceneManagement;
//using System.Threading.Tasks;
//using System;
//using System.Linq;
//using NanoSockets;
//
//public class NetworkRunnerHandler : MonoBehaviour
//{
//    //other components
//    NetworkRunner networkRunner;
//
//    private void Awake()
//    {
//        networkRunner = GetComponent<NetworkRunner>();
//    }
//
//    void Start()
//    {
//        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), null);
//    }
//
//    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
//    {
//        var sceneObjectProvider = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
//
//        if (sceneObjectProvider == null)
//        {
//            //handle network objects already exist in the scene
//            sceneObjectProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
//        }
//
//        //client provides input to host
//        runner.ProvideInput = true;
//
//        return runner.StartGame(new StartGameArgs
//        {
//            GameMode = gameMode,
//            Address = address,
//            Scene = scene,
//            SessionName = "Room",
//            OnGameStarted = initialized,
//            SceneManager = sceneObjectProvider
//        });
//    }
//}