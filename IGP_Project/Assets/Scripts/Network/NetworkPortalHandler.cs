using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
using UnityEngine.SceneManagement;

public class NetworkPortalHandler : MonoBehaviour
{
    public static NetworkPortalHandler Instance;

    private NetworkRunner runner;

    private string nextScene;

    private GameObject[] playerList;

    private bool curInPortal = false;
    private bool allInPortal = false;

    private GameObject portal;

    private void Awake()
    {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        runner = FindObjectOfType<NetworkRunner>();

        curInPortal = false;
        allInPortal = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        //if (portal == null && GameObject.FindWithTag("Portal"))
        //{
        //    foreach(var portalObject in GameObject.FindGameObjectsWithTag("Portal"))
        //    {
        //        if(portalObject.transform.parent == null)
        //        {
        //            portal = portalObject;
        //            break;
        //        }
        //    }
        //    nextScene = portal.GetComponent<PortalScript>().sceneName;
        //}

        if (runner.IsServer && portal != null)
        {
            CheckPlayersPortal();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        allInPortal = false;

        portal = FindPortalObject();
        playerList = FindPlayerObjects();

        if(portal != null)
        {
            nextScene = portal.GetComponent<PortalScript>().sceneName;
        }
    }

    private GameObject FindPortalObject()
    {
        foreach (var portalObject in GameObject.FindGameObjectsWithTag("Portal"))
        {
            if (portalObject.transform.parent == null)
            {
                return portalObject;
            }
        }

        return null;

    }

    private GameObject[] FindPlayerObjects()
    {
        return GameObject.FindGameObjectsWithTag("Player")
                         .Where(player => player.transform.parent == null)
                         .ToArray();
    }

    private void CheckPlayersPortal()
    {
        curInPortal = false;

        if(playerList == null || playerList.Length != GameObject.FindGameObjectsWithTag("Player").Length)
        {
            playerList = FindPlayerObjects();
        }

        //if (playerList != GameObject.FindGameObjectsWithTag("Player")) {
        //    playerList = GameObject.FindGameObjectsWithTag("Player");
        //}

        foreach (var player in playerList)
        {
            if (player.GetComponent<PlayerController>().isInPortal)
            {
                curInPortal = true;
            }
            else
            {
                curInPortal = false;
                break;
            }
        }

        if (curInPortal)
        {
            allInPortal = true;
        }
        
        if (allInPortal)
        {
            Debug.Log("load new Scene");
            allInPortal = false;
            runner.LoadScene(nextScene);
            
        }
    }
}
