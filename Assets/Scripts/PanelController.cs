using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PanelController : MonoBehaviour
{
    public GameObject Panel;
    public GameObject PauseMenu;
    public void OpenPanel()
    {
        Time.timeScale = 0;
        Panel.SetActive(true);
        PauseMenu.SetActive(false);
    }

    public void ClosePanel()
    {
        Time.timeScale = 1;
        Panel.SetActive(false);
    }
}
