using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager instance;

    [HideInInspector] public Dictionary<int, int> itemAmount = new Dictionary<int, int>();
    [HideInInspector] public List<int> itemIDsInInventory = new List<int>();

    public event Action<int> onInventoryChanged;
    public void Start()
    {
        instance = this;

        //AddItemToInventory(0, 2); //Cake
        //AddItemToInventory(4); //Scrap Metal
        //AddItemToInventory(2); //Medicine Box
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddTestItem();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            RemoveTestItem();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ListInventoryItems();
        }
    }
    public void AddItemToInventory(int itemID, int amountToAdd)
    {
        if (itemAmount.ContainsKey(itemID))
        {
            itemAmount[itemID] += amountToAdd;
        }
        else
        {
            itemIDsInInventory.Add(itemID);
            itemIDsInInventory.Sort();
            itemAmount.Add(itemID, amountToAdd);
            PlayerMenuInventoryScreenManager.instance.CreateItemUI();
        }
        NotifySubscribersOfInventoryChange(itemID);
        //onInventoryChanged?.Invoke(itemID); -> also a valid way to test if null
    }
    public void RemoveItemFromInventory(int itemID, int amountToRemove)
    {
        if (itemAmount.ContainsKey(itemID))
        {
            if(itemAmount[itemID] > amountToRemove)
            {
                itemAmount[itemID] -= amountToRemove;
                NotifySubscribersOfInventoryChange(itemID);
            }
            else if (itemAmount[itemID] == amountToRemove)
            {
                itemIDsInInventory.Remove(itemID);
                itemIDsInInventory.Sort();
                itemAmount.Remove(itemID);
                NotifySubscribersOfInventoryChange(itemID);
            }
            else
            {
                Debug.LogWarning("Tried to remove " + amountToRemove + " " +
                    GameItemDictionary.instance.gameItemNames[itemID] + "(s) from player inventory " +
                    "but there are only " + itemAmount[itemID] + " to remove.");
            }
        }
        else
        {
            Debug.LogWarning("Tried to remove an item that does not exist in player inventory.");
        }
    }
    public void NotifySubscribersOfInventoryChange(int itemID)
    {
        if (onInventoryChanged != null)
        {
            onInventoryChanged(itemID);
        }
    }
    //TESTING METHODS
    public void AddTestItem() //Adds random amount of random item to player inventory
    {
        var gameItemDictionary = GameItemDictionary.instance;
        int randomItemID = UnityEngine.Random.Range(0, gameItemDictionary.gameItemNames.Count);
        int randomAmount = UnityEngine.Random.Range(1, 10);
        AddItemToInventory(randomItemID, randomAmount);
        Debug.Log("Added " + randomAmount + " " + GameItemDictionary.instance.gameItemNames[randomItemID] +
            "(s) to player inventory.");
    }
    public void RemoveTestItem()
    {
        var gameItemDictionary = GameItemDictionary.instance;
        int randomItemID = UnityEngine.Random.Range(0, gameItemDictionary.gameItemNames.Count);
        int randomAmount = UnityEngine.Random.Range(1, 10);
        RemoveItemFromInventory(randomItemID, randomAmount);
        Debug.Log("Removed " + randomAmount + " " + GameItemDictionary.instance.gameItemNames[randomItemID] + 
            "(s) from player inventory.");
    }
    public void ListInventoryItems()
    {
        var itemDictionary = GameItemDictionary.instance;
        foreach (int itemID in itemIDsInInventory)
        {
            Debug.Log(itemDictionary.gameItemNames[itemID] + " (" + itemAmount[itemID] + ").");
        }
    }
}
