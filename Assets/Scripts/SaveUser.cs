using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveUser : MonoBehaviour
{
    public string filename;
    public void StoreUser(){
        filename = PlayerPrefs.GetString("filename");
        Debug.Log(filename);
    }

}
