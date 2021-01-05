using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

// controls UpgradeScene
public class UpgradeManager : MonoBehaviour
{
    /* upgrades:
     * 0: health upgrade
     * 1: speed upgrade
     * 2: damage upgrade
     * 3: acceleration upgrade
     */

    // for now, all upgrades cost the same
    public const int UPGRADE_COST = 10;
    // 4 resources
    public const int N_RESOURCES = 4;
    // 4 upgrades
    public const int N_UPGRADES = 4;

    [SerializeField]
    private List<Item> playerInventory;
    [SerializeField]
    private List<int> inventoryCounts;
    [SerializeField]
    private PlayerUpgrades playerUpgrades;
    // upgrade levels that the player started with upon entering this screen
    private int[] minLevels;

    // maps resource indices to index in lists where item is stored
    private int[] resourceIdToInv;

    private static Dictionary<string, int> resourceMap = new Dictionary<string, int>
    {
        { "Chrome", 0 }, { "Gold", 1 }, { "Ruby", 2 }, { "Emerald", 3 }
    };
    // upgrades: tuple item1 is resource id, item2 is count
    private static System.Tuple<int, int>[] upgradeCosts = new System.Tuple<int, int>[N_UPGRADES]
    {
        new System.Tuple<int, int>(0, UPGRADE_COST),
        new System.Tuple<int, int>(1, UPGRADE_COST),
        new System.Tuple<int, int>(2, UPGRADE_COST),
        new System.Tuple<int, int>(3, UPGRADE_COST)
    };

    [SerializeField]
    private GameObject inventoryPanel;
    [SerializeField]
    private GameObject upgradePanel;

    public PlayerData playerData;
    public string filename;
    public int playerLevel;

    [SerializeField]
    private Item[] itemArray;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: load player inventory + upgrades from file
        LoadPlayerDataJson();

        // replace these with the real player values
        // playerInventory = new List<Item>();
        // inventoryCounts = new List<int>();
        // playerUpgrades = new PlayerUpgrades { accelerationLevel = 0, damageLevel = 0, healthLevel = 0, speedLevel = 0 };

        // map resource IDs to positions in inventory
        resourceIdToInv = new int[N_RESOURCES];
        for (int i = 0; i < N_RESOURCES; ++i)
            resourceIdToInv[i] = -1; // -1 means not in inventory
        for (int j = 0; j < playerInventory.Count; ++j)
        {
            int resourceId = resourceMap[playerInventory[j].itemName];
            resourceIdToInv[resourceId] = j;
            UpdateItemCount(resourceId); // initialize menu to show correct inventory count
        }

        minLevels = new int[N_UPGRADES];
        minLevels[0] = playerUpgrades.healthLevel;
        minLevels[1] = playerUpgrades.speedLevel;
        minLevels[2] = playerUpgrades.damageLevel;
        minLevels[3] = playerUpgrades.accelerationLevel;
        for (int i = 0; i < N_UPGRADES; ++i)
            UpdateUpgradePanel(i);
    }

    // Update is called once per frame
    void Update()
    {
    }

    // upgrade the upgrade with index i
    public void Upgrade(int i)
    {
        // find out how many resources we have and how many we need
        int resourceId = upgradeCosts[i].Item1;
        int resouceCost = upgradeCosts[i].Item2;
        int invPos = resourceIdToInv[resourceId];
        int resourceCount;
        if (invPos == -1)
            resourceCount = 0;
        else
            resourceCount = inventoryCounts[invPos];

        if (resourceCount < resouceCost)
            return; // should never happen

        // take resources from player
        inventoryCounts[invPos] -= resouceCost;
        // add upgrade level;
        switch (i)
        {
            case 0:
                playerUpgrades.healthLevel++;
                break;
            case 1:
                playerUpgrades.speedLevel++;
                break;
            case 2:
                playerUpgrades.damageLevel++;
                break;
            case 3:
                playerUpgrades.accelerationLevel++;
                break;
            default:
                throw new System.Exception("Upgrade: invalid upgrade index");
        }

        // update display
        UpdateItemCount(resourceId);
        UpdateUpgradePanel(i);
    }

    // downgrade the upgrade with index i
    public void Downgrade(int i)
    {
        int currentLevel;
        switch (i)
        {
            case 0:
                currentLevel = playerUpgrades.healthLevel;
                break;
            case 1:
                currentLevel = playerUpgrades.speedLevel;
                break;
            case 2:
                currentLevel = playerUpgrades.damageLevel;
                break;
            case 3:
                currentLevel = playerUpgrades.accelerationLevel;
                break;
            default:
                throw new System.Exception("Downgrade: invalid upgrade index");
        }
        if (currentLevel == minLevels[i])
            return; // should never happen

        int resourceId = upgradeCosts[i].Item1;
        int invPos = resourceIdToInv[resourceId];
        if (invPos == -1)
            return; // should never happen
        // give resources back to player
        inventoryCounts[invPos] += upgradeCosts[i].Item2;

        // decrease upgrade level
        switch (i)
        {
            case 0:
                playerUpgrades.healthLevel--;
                break;
            case 1:
                playerUpgrades.speedLevel--;
                break;
            case 2:
                playerUpgrades.damageLevel--;
                break;
            case 3:
                playerUpgrades.accelerationLevel--;
                break;
            default:
                throw new System.Exception("Downgrade: invalid upgrade index");
        }

        UpdateItemCount(resourceId);
        UpdateUpgradePanel(i);
    }

    // update resource inventory count for resource with ID i
    public void UpdateItemCount(int i)
    {
        int invPos = resourceIdToInv[i];
        if (invPos == -1)
        {
            // resource not in inventory i.e. count = 0
            inventoryPanel.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Text>().text = "0";
        }
        else
        {
            inventoryPanel.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Text>().text = System.Convert.ToString(inventoryCounts[invPos]);
        }
    }

    // update upgrade panel for upgrade i
    public void UpdateUpgradePanel(int i)
    {
        Transform panel = upgradePanel.transform.GetChild(i);

        int currentLevel;
        // set currentLevel based on upgrade index
        switch (i)
        {
            case 0:
                currentLevel = playerUpgrades.healthLevel;
                break;
            case 1:
                currentLevel = playerUpgrades.speedLevel;
                break;
            case 2:
                currentLevel = playerUpgrades.damageLevel;
                break;
            case 3:
                currentLevel = playerUpgrades.accelerationLevel;
                break;
            default:
                throw new System.Exception("UpdateUpgradePanel: invalid index");
        }
        panel.GetChild(5).gameObject.GetComponent<Text>().text = System.Convert.ToString(currentLevel);

        // disable downgrade button if we are at minimum upgrade level
        if (currentLevel == minLevels[i])
            panel.GetChild(4).gameObject.GetComponent<Button>().interactable = false;
        else
            panel.GetChild(4).gameObject.GetComponent<Button>().interactable = true;

        // find out how much of the required resource the player has
        int resourceId = upgradeCosts[i].Item1;
        int resourceCost = upgradeCosts[i].Item2;
        int invPos = resourceIdToInv[resourceId];
        int resourceCount;
        if (invPos == -1)
            resourceCount = 0;
        else
            resourceCount = inventoryCounts[invPos];
        // disable upgrade button if we don't have enough resource
        if (resourceCount < resourceCost)
            panel.GetChild(3).gameObject.GetComponent<Button>().interactable = false;
        else
            panel.GetChild(3).gameObject.GetComponent<Button>().interactable = true;
    }

    public void ContinueToLevel()
    {
        // if any resource count has reached 0, the item must be removed from the inventory
        for (int i = inventoryCounts.Count - 1; i >= 0; --i)
        {
            if (inventoryCounts[i] == 0)
            {
                playerInventory.RemoveAt(i);
                inventoryCounts.RemoveAt(i);
            }
        }

        // TODO: save playerUpgrades to player file
        // TODO: save playerInventory and inventoryCounts to player file

        SavePlayerDataJson();

        SceneManager.LoadScene("LoaderScene");
    }

    [ContextMenu("To Json Data")]
    void SavePlayerDataJson()
    {
        filename = PlayerPrefs.GetString("filename");
        playerData.playerUpgrades = playerUpgrades;
        //playerData.playerInventory = playerInventory;
        playerData.items = new List<int>();
        foreach (Item item in playerInventory)
        {
            playerData.items.Add(resourceMap[item.itemName]);
        }
        playerData.itemsNumber = inventoryCounts;
        playerData.level = playerLevel;
        playerData.score = PlayerPrefs.GetInt("score");
        string jsonData = JsonUtility.ToJson(playerData, true);
        Debug.Log("Save");
        Debug.Log(jsonData);
        string path = Application.persistentDataPath + "/" + filename + ".json";
        File.WriteAllText(path, jsonData);
    }

    [ContextMenu("From Json Data")]
    void LoadPlayerDataJson()
    {
        filename = PlayerPrefs.GetString("filename");
        string path = Path.Combine(Application.persistentDataPath + "/" + filename + ".json");
        string jsonData = File.ReadAllText(path);
        Debug.Log("Load");
        Debug.Log(jsonData);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        playerUpgrades = playerData.playerUpgrades;
        playerLevel = playerData.level + 1;
        //playerInventory = playerData.playerInventory;
        playerInventory = new List<Item>();
        foreach (int item in playerData.items)
        {
            playerInventory.Add(itemArray[item]);
        }
        inventoryCounts = playerData.itemsNumber;
    }
}
