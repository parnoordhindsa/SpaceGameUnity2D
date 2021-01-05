using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class pointText : MonoBehaviour
{
    public Text pointTxt;
    public PlayerData playerData;
    public string level;
    public string filename;
    // Start is called before the first frame update
    void Start()
    {
        // LoadPlayerDataJson();   
        pointTxt.text = PlayerPrefs.GetInt("score").ToString();
    }

    void Update() 
    {
        pointTxt.text = PlayerPrefs.GetInt("score").ToString();
    }
}
