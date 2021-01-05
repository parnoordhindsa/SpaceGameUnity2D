using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject PauseMenu;
    // all objects that might be opened from pause menu
    public GameObject[] otherMenus;
    [SerializeField]
    private GameObject minimap;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = PauseMenu.activeSelf;
            if (Time.timeScale == 0.0f){ // game is already paused
                Time.timeScale = 1.0f;
                PauseMenu.SetActive(false);
                foreach (GameObject menu in otherMenus)
                    menu.SetActive(false);
            }else{
                // game is running; pause game
                Time.timeScale = 0.0f;
                PauseMenu.SetActive(true);
            }
        }

        // toggle minimap
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (Time.timeScale == 0.0f) // game is already paused
            {
                if (minimap.activeSelf) // we're already in minimap
                { // unpause the game
                    minimap.SetActive(false);
                    Time.timeScale = 1.0f;
                }
                else // we're in the pause menu or some other non-minimap menu
                {
                    // close other menus
                    foreach (GameObject menu in otherMenus)
                        menu.SetActive(false);
                    PauseMenu.SetActive(false);
                    // open minimap
                    minimap.SetActive(true);
                }
            }
            else // game is running; pause and open minimap
            {
                Time.timeScale = 0.0f;
                // enable minimap
                minimap.SetActive(true);
            }
        }
    }
}
