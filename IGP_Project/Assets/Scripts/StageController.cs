using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class StageController : NetworkBehaviour
{
    public GameObject MonsterPanel;
    public GameObject BoardPanel;
    public GameObject CollectionPanel;
    public GameObject BossPanel;

    private NetworkRunner runner;

    [Networked] private bool On_Monster { get; set; }
    [Networked] private bool On_Board { get; set; }
    [Networked] private bool On_Collection { get; set; }
    //private bool On_Boss = false;

    private ChangeDetector changes;

    private void Start()
    {
        runner = FindObjectOfType<NetworkRunner>();
    }

    public override void Spawned()
    {
        base.Spawned();

        changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

        On_Monster = false;
        On_Board = false;
        On_Collection = false;
    }

    public override void Render()
    {
        base.Render();

        foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(On_Monster):
                    var OnMonsterReader = GetPropertyReader<bool>(nameof(On_Monster));
                    var (prevOnMonster, curOnMonster) = OnMonsterReader.Read(previousBuffer, currentBuffer);
                    OnMonsterChanged(prevOnMonster, curOnMonster);
                    break;

                case nameof(On_Board):
                    var OnBoardReader = GetPropertyReader<bool>(nameof(On_Board));
                    var (prevOnBoard, curOnBoard) = OnBoardReader.Read(previousBuffer, currentBuffer);
                    OnBoardChanged(prevOnBoard, curOnBoard);
                    break;

                case nameof(On_Collection):
                    var OnCollectionReader = GetPropertyReader<bool>(nameof(On_Collection));
                    var (prevOnCollection, curOnCollection) = OnCollectionReader.Read(previousBuffer, currentBuffer);
                    OnCollectionChanged(prevOnCollection, curOnCollection);
                    break;
            }
        }
    }



    public void DrawMonsterStage()
    {
        if (runner.IsServer)
        {
            if (!On_Monster)
            {
                ReSetPanel();
                On_Monster = true;
            }
            else
                On_Monster = false;
            MonsterPanel.gameObject.SetActive(On_Monster);
        }
    }
    public void DrawBoardStage()
    {
        if (runner.IsServer)
        {
            if (!On_Board)
            {
                ReSetPanel();
                On_Board = true;
            }
            else
                On_Board = false;

            BoardPanel.gameObject.SetActive(On_Board);
        }
    }
    public void DrawCollectionStage()
    {
        if (runner.IsServer)
        {
            if (!On_Collection)
            {
                ReSetPanel();
                On_Collection = true;
            }
            else
                On_Collection = false;
            CollectionPanel.gameObject.SetActive(On_Collection);
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
    public void Enter_M2()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("Monster_2");
        }
    }
    public void Enter_M3()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("Monster_3");
        }
    }
    public void Enter_B1()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("Board_1");
        }
    }
    public void Enter_B2()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("Board_2");
        }
    }
    public void Enter_B3()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("Board_3");
        }
    }
    public void Enter_C1()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("Collection_1");
        }
    }
    public void Enter_C2()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("Collection_2");
        }
    }
    public void Enter_C3()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("Collection_3");
        }
    }

    private void ReSetPanel()
    {
        if (On_Monster)
            On_Monster = false;
        else if (On_Board)
            On_Board = false;
        else if (On_Collection)
            On_Collection = false;
        //On_Boss = false;
        MonsterPanel.gameObject.SetActive(On_Monster);
        BoardPanel.gameObject.SetActive(On_Board);
        CollectionPanel.gameObject.SetActive(On_Collection);
        //BossPanel.gameObject.SetActive(false);
    }

    private void OnMonsterChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        On_Monster = newVal;
        MonsterPanel.gameObject.SetActive(On_Monster);
    }

    private void OnBoardChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        On_Board = newVal;
        BoardPanel.gameObject.SetActive(On_Board);
    }

    private void OnCollectionChanged(NetworkBool oldVal, NetworkBool newVal)
    {
        On_Collection = newVal;
        CollectionPanel.gameObject.SetActive(On_Collection);
    }
}
