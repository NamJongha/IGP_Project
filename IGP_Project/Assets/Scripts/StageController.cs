using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class StageController : MonoBehaviour
{
    public GameObject MonsterPanel;
    public GameObject BoardPanel;
    public GameObject CollectionPanel;
    public GameObject BossPanel;

    private NetworkRunner runner;

    private bool On_Monster = false;

    private void Start()
    {
        runner = FindObjectOfType<NetworkRunner>();
    }
    public void DrawMonsterStage()
    {
        if (runner.IsServer)
        {
            if (!On_Monster)
                On_Monster = true;
            else
                On_Monster = false;
            MonsterPanel.gameObject.SetActive(On_Monster);
        }
    }
    public void SeletStage()
    {
        Debug.Log("Select Stage");
        SceneManager.LoadScene("SelectScene");
    }
    public void Enter_M1()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("Monster_1");
        }
    }
}
