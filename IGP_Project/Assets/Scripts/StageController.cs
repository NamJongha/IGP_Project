using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public GameObject MonsterPanel;
    public GameObject BoardPanel;
    public GameObject CollectionPanel;
    public GameObject BossPanel;

    private bool On_Monster = false;
    public void DrawMonsterStage()
    {
        if (!On_Monster)
            On_Monster = true;
        else
            On_Monster = false;
        MonsterPanel.gameObject.SetActive(On_Monster);
    }
    public void SeletStage()
    {
        Debug.Log("Select Stage");
        SceneManager.LoadScene("SelectScene");
    }
    public void Enter_M1()
    {
        SceneManager.LoadScene("Monster_1");
    }
}
