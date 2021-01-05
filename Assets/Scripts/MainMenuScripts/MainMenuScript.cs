using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void QuitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}


[System.Serializable]
public class PlayerData
{
    public string filename;
    public int level;
    public int score;
    public List<int> items;
    public List<int> itemsNumber;
    public PlayerUpgrades playerUpgrades;
}
