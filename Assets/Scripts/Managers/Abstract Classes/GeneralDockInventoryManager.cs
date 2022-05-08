using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralDockInventoryManager : MonoBehaviour
{
    public string shopId;
    [HideInInspector] public Dictionary<int, int> itemAmount = new Dictionary<int, int>();
    [HideInInspector] public List<int> itemIDsInInventory = new List<int>();

    private string sortMode = "Name";
    [HideInInspector] public int storeCash;

    public event Action<int> onInventoryChanged;
    public event Action onInventoryItemAdded;
    public event Action<int> onInventoryItemRemoved;
    public event Action onSortModeChanged;
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
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (sortMode == "Name")
            {
                ChangeSortMode("Value");
                Debug.Log("Sorting inventory by value.");
            }
            else
            {
                ChangeSortMode("Name");
                Debug.Log("Sorting inventory by name.");
            }
        }
    }
    public void AddItemToInventory(int itemID, int amountToAdd)
    {
        if (itemAmount.ContainsKey(itemID))
        {
            itemAmount[itemID] += amountToAdd;
            onInventoryChanged?.Invoke(itemID);
        }
        else
        {
            itemIDsInInventory.Add(itemID);
            SortInventory();
            itemAmount.Add(itemID, amountToAdd);
            onInventoryItemAdded?.Invoke();
        }
    }
    public void RemoveItemFromInventory(int itemID, int amountToRemove)
    {
        if (itemAmount.ContainsKey(itemID))
        {
            if (itemAmount[itemID] > amountToRemove)
            {
                itemAmount[itemID] -= amountToRemove;
                onInventoryChanged?.Invoke(itemID);
            }
            else if (itemAmount[itemID] == amountToRemove)
            {
                itemIDsInInventory.Remove(itemID);
                itemAmount.Remove(itemID);
                onInventoryItemRemoved?.Invoke(itemID);
            }
            else
            {
                Debug.LogWarning("Tried to remove " + amountToRemove + " " +
                    GameItemDictionary.instance.gameItemNames[itemID] + "(s) from " + shopId + " inventory " +
                    "but there are only " + itemAmount[itemID] + " to remove.");
            }
        }
        else
        {
            Debug.LogWarning("Tried to remove an item that does not exist in " + shopId + " inventory.");
        }
    }
    public void ChangeSortMode(string desiredSortMode)
    {
        if (desiredSortMode == "Value")
        {
            sortMode = "Value";
            SortInventory();
            onSortModeChanged?.Invoke();
        }
        else if (desiredSortMode == "Name")
        {
            sortMode = "Name";
            SortInventory();
            onSortModeChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning("Desired sort mode does not exist. Sorting by default.");
            sortMode = "Name";
            itemIDsInInventory.Sort();
            onSortModeChanged?.Invoke();
        }
    }
    public void SortInventory()
    {
        if (itemIDsInInventory.Count > 1)
        {
            if (sortMode == "Value")
            {
                SortByValue();
            }
            else
            {
                itemIDsInInventory.Sort();
            }
        }
        else
        {
            Debug.LogWarning("Tried to sort " + shopId + " inventory, but inventory does not have enough items to be sorted");
        }
    }
    public void SortByValue()
    {
        int buffer = 0;
        List<int> bufferInventory = new List<int>();
        bool sortComplete = false;
        while (!sortComplete)
        {
            buffer = 0;
            foreach (int itemID in itemIDsInInventory)
            {
                if (bufferInventory.Contains(itemID)) { continue; }
                if (bufferInventory.Contains(buffer) || !itemIDsInInventory.Contains(buffer))
                {
                    buffer = itemID;
                }
                if (GameItemDictionary.instance.gameItemValues[itemID] > GameItemDictionary.instance.gameItemValues[buffer])
                {
                    buffer = itemID;
                }
            }
            bufferInventory.Add(buffer);
            if (itemIDsInInventory.Count == bufferInventory.Count)
            {
                sortComplete = true;
            }
        }
        for (int i = 0; i < bufferInventory.Count; i++)
        {
            itemIDsInInventory[i] = bufferInventory[i];
        }
    }
    //TESTING METHODS
    public void AddTestItem() //Adds random amount of random item to player inventory
    {
        var gameItemDictionary = GameItemDictionary.instance;
        int randomItemID = UnityEngine.Random.Range(0, gameItemDictionary.gameItemNames.Count);
        int randomAmount = UnityEngine.Random.Range(1, 10);
        //AddItemToInventory(randomItemID, randomAmount);
        AddItemToInventory(randomItemID, randomAmount);
        Debug.Log("Added " + randomAmount + " " + GameItemDictionary.instance.gameItemNames[randomItemID] +
            "(s) to " + shopId + " inventory.");
    }
    public void RemoveTestItem()
    {
        var gameItemDictionary = GameItemDictionary.instance;
        int randomItemID = UnityEngine.Random.Range(0, gameItemDictionary.gameItemNames.Count);
        Debug.Log(gameItemDictionary.gameItemNames.Count - 1);
        int randomAmount = UnityEngine.Random.Range(1, 10);
        RemoveItemFromInventory(randomItemID, randomAmount);
        Debug.Log("Removed " + randomAmount + " " + GameItemDictionary.instance.gameItemNames[randomItemID] +
            "(s) from" + shopId + " inventory.");
    }
}
