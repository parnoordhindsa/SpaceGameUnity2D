using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;


public class SignInMenu : MonoBehaviour
{
    public string theFilename;
    public GameObject userInputField;
    public GameObject textDisplay;
    public PlayerData playerData;

    public void LoadFile()
    {
        theFilename = userInputField.GetComponent<Text>().text;

        try{
            LoadPlayerDataJson();
        }
        catch(FileNotFoundException){
            textDisplay.GetComponent<Text>().text = "File : " + theFilename + " Does Not exists!";                
        }
    }

    [ContextMenu("From Json Data")]
    void LoadPlayerDataJson()
    {
        string path = Path.Combine(Application.persistentDataPath + "/" + theFilename + ".json");
        string jsonData = File.ReadAllText(path);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        PlayerPrefs.SetInt("score", playerData.score);
        PlayerPrefs.SetString("filename", playerData.filename); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }
}
