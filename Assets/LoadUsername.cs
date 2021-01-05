using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadUsername : MonoBehaviour
{
    private TMP_Text userText;
    public string username;
    void Start()
    {
        username = PlayerPrefs.GetString("username");
        userText = GetComponent<TMP_Text>();
        userText.text = username;
    }

}
