using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayerTeleport : MonoBehaviour
{
    public int score;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Wormhole wormhole = collision.collider.gameObject.GetComponent<Wormhole>();
        if (wormhole is null) return;
        if (wormhole.GetTravel())
        {
            score = PlayerPrefs.GetInt("score") + 100;
            Debug.Log(score);
            PlayerPrefs.SetInt("score", score);

            print("Entered wormhole!");
            Level level = GameObject.FindWithTag("Level").GetComponent<Level>();
            level.NextLevel();
        }
    }
}
