using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string nextLevel;

    public void MoveToNextLevel()
    {
        Debug.Log("Next Stage");
        SceneManager.LoadScene(nextLevel);
    }

    
}