using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public struct PlayerUpgrades
{
    // level of max health upgrade
    public int healthLevel;
    // level of max speed upgrade
    public int speedLevel;
    // level of laser damage upgrade
    public int damageLevel;
    // level of acceleration upgrade
    public int accelerationLevel;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isPaused;

    public List<Item> items = new List<Item>(); // what kind of items we have
    public List<int> itemsNumber = new List<int>(); // how manny items we have
    public GameObject[] slots;

    public ItemButton itemButton;
    public ItemButton[] itemButtons;

    public PlayerData playerData;
    public string filename;

    [SerializeField]
    private Item[] itemReferences;

    private InventoryManager inventoryManager;

    private static Dictionary<string, int> resourceMap = new Dictionary<string, int>
    {
        { "Chrome", 0 }, { "Gold", 1 }, { "Ruby", 2 }, { "Emerald", 3 }
    };

    private void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            if (instance != this){
                Destroy(gameObject);
            }
        }
        // DontDestroyOnLoad(gameObject);
    }

    private void Start(){
        LoadPlayerDataJson();
        DisplayItems();
    }

    public void Initialize(InventoryManager inventory)
    {
        inventoryManager = inventory;
    }

    [ContextMenu("From Json Data")]
    void LoadPlayerDataJson()
    {
        filename = PlayerPrefs.GetString("filename");
        string path = Path.Combine(Application.persistentDataPath + "/" + filename + ".json");
        string jsonData = File.ReadAllText(path);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        //items = playerData.items;
        items = new List<Item>();
        foreach (int item in playerData.items)
        {
            items.Add(itemReferences[item]);
        }
        itemsNumber = playerData.itemsNumber;
    }

    [ContextMenu("To Json Data")]
    void SavePlayerDataJson()
    {
        // Debug.Log("here");
        playerData.filename = filename;
        //playerData.items = items;
        playerData.items = new List<int>();
        foreach (Item item in items)
        {
            playerData.items.Add(resourceMap[item.itemName]);
        }
        playerData.itemsNumber = itemsNumber;
        // string jsonData = JsonUtility.ToJson(playerData, true);
        // string path = Application.persistentDataPath + "/" + filename + ".json";
        // File.WriteAllText(path, jsonData);
    }

    // not used
    private void DisplayItems(){
        for (int i = 0; i < slots.Length; i++){
            if (i < items.Count){
                slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1,1,1,1);
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].itemSprite;

                // update slot count text
                slots[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1,1,1,1);
                slots[i].transform.GetChild(1).GetComponent<Text>().text = itemsNumber[i].ToString();

                // update close and throw button
                slots[i].transform.GetChild(2).gameObject.SetActive(true);
            }
            else{
                slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1,1,1,0);
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;

                // update slot count text
                slots[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1,1,1,0);
                slots[i].transform.GetChild(1).GetComponent<Text>().text =  null;

                // update close and throw button
                slots[i].transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    public void AddItem(Item _item){
        if (!items.Contains(_item))
        {
            items.Add(_item);
            itemsNumber.Add(1);

            SavePlayerDataJson();
        }
        else{
            for (int i = 0; i < items.Count; i++){
                if (_item == items[i]){
                    itemsNumber[i]++;
                }
            }
        }

        // DisplayItems();
    }

    // not used
    public void RemoveItem(Item _item){
        if (items.Contains(_item)){
            for (int i = 0; i < items.Count; i++){
                if (_item == items[i]){
                    itemsNumber[i]--;
                    if (itemsNumber[i] == 0){
                        items.Remove(_item);
                        itemsNumber.Remove(itemsNumber[i]);
                    }
                }
            }
            SavePlayerDataJson();
        }
        else{
            // Debug.Log("There is no " + _item + " in this inventory");
        }

        ResetButtonItems();
        DisplayItems();
    }

    // not used
    public void ResetButtonItems(){
        for (int i = 0; i < itemButtons.Length; i++){
            if (i < items.Count){
                itemButtons[i].thisItem = items[i];
            }
            else{
                itemButtons[i].thisItem = null;
            }
        }
    }
}
