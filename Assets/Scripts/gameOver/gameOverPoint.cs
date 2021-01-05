using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class gameOverPoint : MonoBehaviour
{

    public Text pointTxt;
    public string point;
    // Start is called before the first frame update
    void Start()
    {   
        Debug.Log(PlayerPrefs.GetInt("score"));
        point = PlayerPrefs.GetInt("score").ToString();
        // LoadPlayerDataJson();   
        pointTxt.text = "Points : " + point;
    }
}
