using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class levelText : MonoBehaviour
{
    public Text lvlText;
    public PlayerData playerData;
    public string level;
    public string filename;
    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerDataJson();   
        lvlText.text = "Level : " + level;
    }

    [ContextMenu("From Json Data")]
    void LoadPlayerDataJson()
    {
        filename = PlayerPrefs.GetString("filename");
        string path = Path.Combine(Application.persistentDataPath + "/" + filename + ".json");
        string jsonData = File.ReadAllText(path);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        level = (playerData.level + 1).ToString();
    }

}
