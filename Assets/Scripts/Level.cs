using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField]
    GameObject levelPrefab;
    [SerializeField]
    GameObject planetPrefab;
    [SerializeField]
    GameObject wormholePrefab;
    [SerializeField]
    GameObject wormholePlanetPrefab;
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    GameObject cameraPrefab;
    [SerializeField]
    GameObject asteroidPrefab;
    [SerializeField]
    GameObject borderAsteroidPrefab;
    [SerializeField]
    GameObject enemyPrefab;
    [SerializeField]
    GameObject eventSystemPrefab;
    [SerializeField]
    GameObject gameManagerPrefab;
    [SerializeField]
    GameObject canvasPrefab;
    [SerializeField]
    GameObject inventoryPanelPrefab;
    [SerializeField]
    GameObject tooltipPrefab;

    // resource loot tables
    // used for initialization in Unity
    [SerializeField]
    private GameObject[] PlanetDropArray;
    // actual loot table for objects to use
    public LootTable<GameObject> PlanetDrops;

    [SerializeField]
    private GameObject[] EnemyDropArray;
    public LootTable<GameObject> EnemyDrops;

    public int Difficulty;
    public float Radius;

    public const float ASTEROID_BASE_RADIUS = 0.71f;
    public const float PLANET_BASE_RADIUS = 3.52f;

    public GameObject gameManager;

    public string username;
    public PlayerData playerData;

    public string filename;

    private static Dictionary<string, int> resourceMap = new Dictionary<string, int>
    {
        { "Chrome", 0 }, { "Gold", 1 }, { "Ruby", 2 }, { "Emerald", 3 }
    };
    void Start()
    {
        try{
            LoadPlayerDataJson();
        }catch(FileNotFoundException){
            // SavePlayerDataJson();
        }
    }

    [ContextMenu("From Json Data")]
    void LoadPlayerDataJson()
    {
        username = PlayerPrefs.GetString("filename");
        string path = Path.Combine(Application.persistentDataPath + "/" + username + ".json");
        string jsonData = File.ReadAllText(path);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void logout(string mainMenu)
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void Initialize(LevelData data)
    {
        LoadPlayerDataJson();
        // radius of the background circle;
        Radius = data.radius;
        System.Random random = new System.Random();

        // set up loot tables
        PlanetDrops = new LootTable<GameObject>();
        foreach (GameObject o in PlanetDropArray)
        {
            PlanetDrops.Add(o, 1.0f);
        }
        EnemyDrops = new LootTable<GameObject>();
        foreach (GameObject o in EnemyDropArray)
        {
            EnemyDrops.Add(o, 1.0f);
        }

        PlayerUpgrades playerUpgrades = playerData.playerUpgrades;

        GameObject temp;
        Wormhole wormhole;
        GameObject player;
        // some objects need their scale changed before being safely inserted into the level
        Vector2 loadingArea = new Vector2(data.radius * 2, 0);

        temp = Instantiate(wormholePrefab, data.wormhole.Pos, Quaternion.identity);
        wormhole = temp.GetComponent<Wormhole>();
        wormhole.Initialize(false);

        foreach (PlanetData planetData in data.planets)
        {
            if (planetData.Wormhole)
                temp = Instantiate(wormholePlanetPrefab, loadingArea, Quaternion.AngleAxis((float)random.NextDouble() * 2 * Mathf.PI, Vector3.forward));
            else
                temp = Instantiate(planetPrefab, loadingArea, Quaternion.AngleAxis((float)random.NextDouble() * 360.0f, Vector3.forward));
            temp.transform.localScale *= planetData.Radius / PLANET_BASE_RADIUS;
            temp.transform.position = planetData.Pos;
        }

        GameObject camera = Instantiate(cameraPrefab);
        player = Instantiate(playerPrefab, data.playerPos, Quaternion.identity);
        gameManager = Instantiate(gameManagerPrefab);
        GameObject canvas = Instantiate(canvasPrefab);
        /*
        GameObject inventoryPanel = Instantiate(inventoryPanelPrefab);
        inventoryPanel.transform.SetParent(canvas.transform.GetChild(1));
        for (int i = 0; i < 14; ++i)
        {
            Instantiate(gameManager.transform.GetChild(i)).transform.SetParent(inventoryPanel.transform);
        }
        Instantiate(tooltipPrefab).transform.SetParent(canvas.transform.GetChild(1));
        */
        canvas.transform.GetChild(8).gameObject.GetComponent<InventoryManager>().Initialize();
        gameManager.GetComponent<GameManager>().Initialize(canvas.transform.GetChild(8).gameObject.GetComponent<InventoryManager>());

        // initialize minimap with planet data
        canvas.transform.GetChild(4).gameObject.GetComponent<Minimap>().Initialize(data.planets);

        // initialize player upgrades
        player.GetComponent<PlayerM>().Initialize(canvas.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Slider>(), playerUpgrades);
        player.GetComponent<PlayerShooting>().Initialize(playerUpgrades);
        // tell camera to follow player
        camera.GetComponent<CameraMovement>().Initialize(player);

        temp = Instantiate(eventSystemPrefab);

        foreach (AsteroidData asteroidData in data.borderAsteroids)
        {
            temp = Instantiate(borderAsteroidPrefab, loadingArea, Quaternion.AngleAxis((float)random.NextDouble() * 360.0f, Vector3.forward));
            temp.transform.localScale *= asteroidData.Radius / ASTEROID_BASE_RADIUS;
            temp.transform.position = asteroidData.Pos;
        }

        foreach (AsteroidData asteroidData in data.fieldAsteroids)
        {
            temp = Instantiate(asteroidPrefab, loadingArea, Quaternion.AngleAxis((float)random.NextDouble() * 360.0f, Vector3.forward));
            temp.transform.localScale *= asteroidData.Radius / ASTEROID_BASE_RADIUS;
            temp.transform.position = asteroidData.Pos;
            temp.GetComponent<Asteroid>().Initialize(asteroidData.Direction);
        }

        foreach (EnemyData enemyData in data.enemies)
        {
            temp = Instantiate(enemyPrefab, enemyData.Pos, Quaternion.identity);
            temp.GetComponent<EnemyMovement>().CombatStats(enemyData.Level);
        }

        if (playerData.level == 0)
        {
            canvas.transform.GetChild(3).GetChild(4).gameObject.GetComponent<PanelController>().OpenPanel();
        }
    }

    public void NextLevel()
    {
        // gameManager.GetComponent<GameManager>().items
        SavePlayerDataJson();
        SceneManager.LoadScene("UpgradeScene");
        
    }

    [ContextMenu("To Json Data")]
    void SavePlayerDataJson()
    {
        filename = PlayerPrefs.GetString("filename");
        string jsonData = JsonUtility.ToJson(gameManager.GetComponent<GameManager>().playerData, true);
        string path = Application.persistentDataPath + "/" + filename + ".json";
        File.WriteAllText(path, jsonData);
    }

}
