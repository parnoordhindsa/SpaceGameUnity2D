using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

public class Scoreboard : MonoBehaviour
{
    public PlayerData playerData;
    public List<PlayerData> playerList;

    public int playerCount;
    public int counter;

    public Canvas canvas;

    public PlayerData maxPlayer;
    public int maxScore;
    public List<PlayerData> bestPlayers;
    public int countExep;
    public List<GameObject> textList;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyChildren()
    {
        foreach(GameObject txt in textList) {
            Destroy(txt);
        }
        bestPlayers.Clear();
        playerList.Clear();
    }

    public void LoadScores() 
    {
        string folderPath = Path.Combine(Application.persistentDataPath);

        // System.IO.DirectoryInfo di = new DirectoryInfo(folderPath);
        // foreach(FileInfo file in di.GetFiles())
        // {
        //     file.Delete();
        // }

        foreach (string file in Directory.EnumerateFiles(folderPath, "*.json"))
        {
            string jsonData = File.ReadAllText(file);
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            playerList.Add(playerData);
        }

        countExep = playerList.Count;

        if (playerList.Count < 5) {
            for (var i=0; i<countExep; i++) {
                maxScore = playerList.Max(x => x.score);
                maxPlayer = playerList.First(x => x.score == maxScore);

                bestPlayers.Add(maxPlayer);
                playerList.Remove(maxPlayer);
            }
        }else{
            for (var i=0; i<5; i++) {
                maxScore = playerList.Max(x => x.score);
                maxPlayer = playerList.First(x => x.score == maxScore);

                bestPlayers.Add(maxPlayer);
                playerList.Remove(maxPlayer);
            }
        }

        if(countExep == 0) {
            GameObject newText = new GameObject("NoPlayer");
            textList.Add(newText);
            Text addText = newText.AddComponent<Text>();
            RectTransform rt = newText.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1200, 200);
            addText.text = "No Player Data Found";
            addText.fontSize = 100;
            addText.color = Color.yellow;
            addText.fontStyle = FontStyle.Bold;
            addText.transform.SetParent(transform);
            Font ArialFont = (Font)Resources.GetBuiltinResource (typeof(Font), "Arial.ttf");
            addText.font = ArialFont;
            addText.alignment = TextAnchor.MiddleCenter;
            // addText.anchor = TextAnchor.UpperRight;

            addText.transform.position = new Vector3(1920/2.0f, 600.0f, 0.0f);

            addText.transform.SetParent(transform);

        }else{
            playerCount = playerList.Count;

            counter = 5;

            foreach (PlayerData player in bestPlayers) {
                Debug.Log(counter);
                GameObject newText = new GameObject(player.filename);
                
                textList.Add(newText);
                
                Text addText = newText.AddComponent<Text>();

                RectTransform rt = newText.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(1200, 200);

                if(counter == 5) {   
                    addText.text = "Best Player : " + player.filename + " ( " + player.score + " ) ";
                    addText.fontSize = 80;
                    addText.color = Color.yellow;
                    addText.fontStyle = FontStyle.Bold;
                    
                }else{
                    if(counter == 4) {
                        addText.text = "2nd : " + player.filename + " ( " + player.score + " ) ";
                    }else if (counter == 3) {
                        addText.text = "3rd : " + player.filename + " ( " + player.score + " ) ";
                    }else if (counter == 2) {
                        addText.text = "4th : " + player.filename + " ( " + player.score + " ) ";
                    }else if (counter == 1) {
                        addText.text = "5th : " + player.filename + " ( " + player.score + " ) ";
                    }
                    addText.fontSize = 50;
                    addText.color = Color.green;
                }
                
                addText.transform.SetParent(transform);
                Font ArialFont = (Font)Resources.GetBuiltinResource (typeof(Font), "Arial.ttf");
                addText.font = ArialFont;
                addText.alignment = TextAnchor.MiddleCenter;
                // addText.anchor = TextAnchor.UpperRight;

                addText.transform.position = new Vector3(1920/2.0f, counter * 150.0f, 0.0f);

                addText.transform.SetParent(transform);
                counter--;
            }
        }
    }


}
