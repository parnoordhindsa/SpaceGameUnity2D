using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class SignUpMenu : MonoBehaviour
{
    public string theFilename;
    public GameObject userInputField;
    public PlayerData playerData;
    public GameObject textDisplay;


    public void StoreName()
    {
        // theFilename = userInputField.GetComponent<Text>().text;
        // playerData.filename = theFilename;
        // SavePlayerDataJson();
        theFilename = userInputField.GetComponent<Text>().text;
        try{
            LoadPlayerDataJson();
        }catch(FileNotFoundException){
            playerData.filename = theFilename;
            playerData.level = 0;
            playerData.score = 0;
            SavePlayerDataJson();
        }
    }

    [ContextMenu("To Json Data")]
    void SavePlayerDataJson()
    {
        string jsonData = JsonUtility.ToJson(playerData, true);
        string path = Application.persistentDataPath + "/" + theFilename + ".json";
        File.WriteAllText(path, jsonData);

        PlayerPrefs.SetString("filename", playerData.filename);
        PlayerPrefs.SetInt("score", playerData.score);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    [ContextMenu("From Json Data")]
    void LoadPlayerDataJson()
    {
        string path = Path.Combine(Application.persistentDataPath + "/" + theFilename + ".json");
        string jsonData = File.ReadAllText(path);
        textDisplay.GetComponent<Text>().text = "File : " + theFilename + " exists!";                
    }
}
