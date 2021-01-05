using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    // unity fields - images and descriptions of tutorial scenes
    [SerializeField]
    private Sprite[] scenes;
    [SerializeField]
    private string[] descriptions;
    [SerializeField]
    private GameObject nextScene;
    [SerializeField]
    private GameObject prevScene;

    // other elements of the scene
    [SerializeField]
    private Image display;
    [SerializeField]
    private GameObject text;

    // which scene are we displaying?
    private int scene;

    // Start is called before the first frame update
    void Start()
    {
        if (scenes.Length != descriptions.Length)
            throw new System.Exception("# of tutorial scenes must == # of descriptions!");
        else if (scenes.Length == 0)
            throw new System.Exception("Tutorial requires example scenes!");
        scene = 0;
        UpdateScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // update displayed tutorial information
    private void UpdateScene()
    {
        if (scene < 0 || scene >= scenes.Length)
            throw new System.Exception("Tutorial scene # out of range!");
        display.sprite = scenes[scene];
        text.GetComponent<TextMeshProUGUI>().text = descriptions[scene];
        if (scene == 0)
        {
            prevScene.SetActive(false);
            nextScene.SetActive(true);
        }
        else if (scene == scenes.Length - 1)
        {
            prevScene.SetActive(true);
            nextScene.SetActive(false);
        }
        else
        {
            prevScene.SetActive(true);
            nextScene.SetActive(true);
        }
    }

    public void NextScene()
    {
        // increment scene # if possible
        scene = Math.Min(scenes.Length - 1, scene + 1);
        UpdateScene();
    }

    public void PrevScene()
    {
        // decrement scene # if possible
        scene = Math.Max(0, scene - 1);
        UpdateScene();
    }
}
