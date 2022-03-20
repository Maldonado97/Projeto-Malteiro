using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;
    public Dictionary<int, int> itemAmount = new Dictionary<int, int>();
    public List<int> itemIDsInInventory = new List<int>();
    public void Start()
    {
        instance = this;

        AddItemToInventory(0); //Cake
        AddItemToInventory(4); //Scrap Metal
        AddItemToInventory(2); //Medicine Box
        ListInventoryItems();
    }
    public void AddItemToInventory(int itemID)
    {
        if (itemAmount.ContainsKey(itemID))
        {
            itemAmount[itemID] += 1;
        }
        else
        {
            itemIDsInInventory.Add(itemID);
            itemIDsInInventory.Sort();
            itemAmount.Add(itemID, 1);
        }
    }
    public void ListInventoryItems()
    {
        var itemDictionary = GameItemDictionary.instance;
        foreach(int itemID in itemIDsInInventory)
        {
            Debug.Log(itemDictionary.gameItemNames[itemID] + " (" + itemAmount[itemID] + ").");
        }
    }
}
