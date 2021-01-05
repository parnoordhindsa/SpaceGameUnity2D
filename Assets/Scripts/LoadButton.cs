using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// called LoadButton for legacy reasons; now just manages loading screen (and transitioning to level)
public class LoadButton : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingText;
    private LevelGenerator levelGenerator = null;
    private Task<int> generateTask = null;
    [SerializeField]
    private GameObject levelPrefab;

    public const float LEVEL_RADIUS = 200.0f;

    // Start is called before the first frame update
    void Start()
    {
        // begin generating level
        int level = GetCurrentLevel();
        levelGenerator = new LevelGenerator(LEVEL_RADIUS + Mathf.Sqrt(level * 1600), level);
        generateTask = levelGenerator.Generate();
    }

    private void TransitionToLevel()
    {
        Destroy(Camera.main.gameObject);
        Destroy(loadingText);

        Scene scene = SceneManager.CreateScene("Level");
        if (!scene.IsValid())
            throw new Exception("Problem creating new scene!");
        SceneManager.SetActiveScene(scene);
        GameObject level = Instantiate(levelPrefab);
        SceneManager.MoveGameObjectToScene(levelPrefab, scene);
        level.GetComponent<Level>().Initialize(levelGenerator.GetLevelData());

        Destroy(gameObject);
    }

    // loads current save file (using PlayerPrefs) and returns the current level
    private int GetCurrentLevel()
    {
        string username = PlayerPrefs.GetString("filename");
        string path = Path.Combine(Application.persistentDataPath + "/" + username + ".json");
        string jsonData = File.ReadAllText(path);
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        return playerData.level;
    }

    // Update is called once per frame
    void Update()
    {
        // wait until level generation has completed
        if (!(generateTask is null) && generateTask.IsCompleted)
        {
            if (generateTask.Result != 0)
                throw new Exception("Failed to create level: error code " + Convert.ToString(generateTask.Result));

            /*
            loadingText.SetActive(false);
            TextMeshPro text = GetComponent<TextMeshPro>();
            text.text = "Play"; */
            TransitionToLevel();
        }
    }

    /*
    public void OnMouseDown()
    {
        if (levelGenerator is null)
        {
            levelGenerator = new LevelGenerator(LEVEL_RADIUS);
            generateTask = levelGenerator.Generate();

            loadingText.SetActive(true);
        }
        else if (!(generateTask is null) && generateTask.IsCompleted)
        {
            TransitionToLevel();
        }
    }
    */
}
