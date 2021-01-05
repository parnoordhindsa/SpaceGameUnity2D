using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class gameOver : MonoBehaviour
{
    public void BackToMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
