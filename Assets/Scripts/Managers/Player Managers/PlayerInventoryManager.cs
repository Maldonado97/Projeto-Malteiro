using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager instance;

    [Header("TESTING")]
    [SerializeField] bool testingModeActive = false;
    [HideInInspector] public Dictionary<int, int> itemAmount = new Dictionary<int, int>();
    [HideInInspector] public List<int> itemIDsInInventory = new List<int>();
    [HideInInspector] public List<int> fuelItemsInInventory = new List<int>();
    [HideInInspector] public float playerCash;
    [HideInInspector] public float totalWeight = 0;
    [HideInInspector] public float maxWeight = 1000; //Initialization isn't working for some reason, so i'm setting the value on the awake method.
    [HideInInspector] public float maxFuel = 900; //Consumed by player control script
    [HideInInspector] public float fuel = 0;

    private string sortMode = "Name";

    public event Action<int> onInventoryChanged;
    public event Action onInventoryItemAdded;
    public event Action<int> onInventoryItemRemoved;
    public event Action onInventoryWeightChanged;
    public event Action onInventoryCashChanged;
    public event Action onSortModeChanged;
    public void Awake()
    {
        instance = this;
        maxWeight = 25000;
        maxFuel = 900;
        fuel = 900;
        playerCash = 2000;
        //Debug.Log($"Player inventory Max Weight is: {maxWeight}");
    }
    private void Update()
    {
        if (testingModeActive)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                AddTestItem();
                //if(fuel + 300 !> maxFuel)
                //{
                    //fuel += 300;
                //}
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                RemoveTestItem();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                ListInventoryItems();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (sortMode == "Name")
                {
                    ChangeSortMode("Value");
                    //Debug.Log("Sorting inventory by value.");
                }
                else
                {
                    ChangeSortMode("Name");
                    //Debug.Log("Sorting inventory by name.");
                }
            }
        }
    }
    //INVENTORY MANAGEMENT
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
            if (GameItemDictionary.instance.gameItemTypes[itemID] == "Fuel")
            {
                fuelItemsInInventory.Add(itemID);
            }
            SortInventory();
            itemAmount.Add(itemID, amountToAdd);
            onInventoryItemAdded?.Invoke();
        }
        AddInventoryWeight(itemID, amountToAdd);
        onInventoryWeightChanged?.Invoke();
    }
    public void RemoveItemFromInventory(int itemID, int amountToRemove)
    {
        if (itemAmount.ContainsKey(itemID))
        {
            if(itemAmount[itemID] < amountToRemove)
            {
                Debug.LogWarning("Tried to remove " + amountToRemove + " " +
                    GameItemDictionary.instance.gameItemNames[itemID] + "(s) from player inventory " +
                    "but there are only " + itemAmount[itemID] + " to remove.");
            }
            else
            {
                if (itemAmount[itemID] > amountToRemove)
                {
                    itemAmount[itemID] -= amountToRemove;
                    onInventoryChanged?.Invoke(itemID);

                }
                else if (itemAmount[itemID] == amountToRemove)
                {
                    itemIDsInInventory.Remove(itemID);
                    if (GameItemDictionary.instance.gameItemTypes[itemID] == "Fuel")
                    {
                        fuelItemsInInventory.Remove(itemID);
                    }
                    itemAmount.Remove(itemID);
                    onInventoryItemRemoved?.Invoke(itemID);
                }
                RemoveInventoryWeight(itemID, amountToRemove);
                onInventoryWeightChanged?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning("Tried to remove an item that does not exist in player inventory.");
        }
    }
    public void AddInventoryWeight(int itemID, int itemAmount)
    {
        float itemWeight = GameItemDictionary.instance.gameItemWeights[itemID];
        totalWeight += itemWeight * itemAmount;
    }
    public void RemoveInventoryWeight(int itemID, int itemAmount)
    {
        float itemWeight = GameItemDictionary.instance.gameItemWeights[itemID];
        totalWeight -= itemWeight * itemAmount;
    }
    public void OnInventoryCashChanged()
    {
        onInventoryCashChanged?.Invoke();
    }
    //SORTING
    public void ChangeSortMode(string desiredSortMode)
    {
        if(desiredSortMode == "Value")
        {
            sortMode = "Value";
            SortInventory();
            onSortModeChanged?.Invoke();
        }
        else if(desiredSortMode == "Name")
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
            //Debug.LogWarning("Tried to sort inventory, but inventory does not have enough items to be sorted");
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
        //onSortModeChanged?.Invoke();
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
            "(s) to player inventory.");
    }
    public void RemoveTestItem()
    {
        var gameItemDictionary = GameItemDictionary.instance;
        int randomItemID = UnityEngine.Random.Range(0, gameItemDictionary.gameItemNames.Count);
        Debug.Log(gameItemDictionary.gameItemNames.Count - 1);
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
