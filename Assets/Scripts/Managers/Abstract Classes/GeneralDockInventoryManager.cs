using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralDockInventoryManager : MonoBehaviour
{
    public string shopId;
    [HideInInspector] public bool startingNewGame = true;
    //INVENTORY LISTS
    [HideInInspector] public Dictionary<int, int> itemAmount = new Dictionary<int, int>();
    [HideInInspector] public List<int> itemIDsInInventory = new List<int>();
    [HideInInspector] public List<int> fuelItemsInInventory = new List<int>();
    [HideInInspector] public List<int> mirrorPlayerInventory = new List<int>();
    [HideInInspector] public List<int> mirrorCustomSubPlayerInventory = new List<int>();
    protected List<int> customPlayerSubInventoryAllowedItems = new List<int>();
    //SORTING
    private string sortMode = "Name";
    private string mirrorInventorySortMode = "Name";
    //SHUFFLING
    public bool canShuffleInventory = true;
    protected List<int> allowedItemTypes = new List<int>(); //USE THIS TO LIMIT ITEM SHUFFLER (NOT IMPLEMENTED YET)
    protected int desiredInventoryItems = 3;
    protected float desiredInventoryItemsVariance = .4f;
    protected int desiredCash = 12000;
    //MISC
    [HideInInspector] public float storeCash;
    public bool testingModeActive = false;

    public event Action<int> onInventoryChanged;
    public event Action onInventoryItemAdded;
    public event Action<int> onInventoryItemRemoved;
    public event Action<int> onMirrorInventoryItemAdded;
    public event Action<int> onMirrorInventoryItemRemoved;
    public event Action onInventoryCashChanged;
    public event Action onSortModeChanged;
    public event Action onMirrorInventorySortModeChanged;
    protected void Awake()
    {
        SetInstance();
    }
    public virtual void Start()
    {
        SetStartingCash();
        StartCoroutine(StartInventoryShuffleCounter(180));
        //StartCoroutine(StartInventoryShuffleCounter(1));
        if (startingNewGame)
        {
            AddInitialItems();
        }

        PlayerInventoryManager.instance.onInventoryItemAdded += AddItemToMirrorInventory;
        PlayerInventoryManager.instance.onInventoryItemRemoved += RemoveItemFromMirrorPlayerInventory;
        CreateMirrorPlayerInventory();
    }
    protected void Update()
    {
        if (testingModeActive)
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
                Debug.Log($"Changing {shopId} sortmode");
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
    }
    protected abstract void SetInstance();
    protected abstract void SetStartingCash();
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
            if(GameItemDictionary.instance.gameItemTypes[itemID] == "Fuel")
            {
                fuelItemsInInventory.Add(itemID); //THIS NEEDS TO BE SORTED AS WELL. ADD CODE FOR IT
            }
            SortInventory(itemIDsInInventory, sortMode);
            itemAmount.Add(itemID, amountToAdd);
            onInventoryItemAdded?.Invoke();
        }
        //Debug.Log($"{amountToAdd} {GameItemDictionary.instance.gameItemNames[itemID]}(s) added.");
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
                if (GameItemDictionary.instance.gameItemTypes[itemID] == "Fuel")
                {
                    fuelItemsInInventory.Remove(itemID);
                }
                itemAmount.Remove(itemID);
                onInventoryItemRemoved?.Invoke(itemID);
            }
            else
            {
                Debug.LogWarning("Tried to remove " + amountToRemove + " " +
                    GameItemDictionary.instance.gameItemNames[itemID] + "(s) from " + shopId + " inventory " +
                    "but there are only " + itemAmount[itemID] + " to remove.");
            }
            //Debug.Log($"{amountToRemove} {GameItemDictionary.instance.gameItemNames[itemID]}(s) removed.");
        }
        else
        {
            Debug.LogWarning("Tried to remove an item that does not exist in " + shopId + " inventory.");
        }
    }
    public void CreateMirrorPlayerInventory()
    {
        foreach (int itemID in PlayerInventoryManager.instance.itemIDsInInventory)
        {
            if (!mirrorPlayerInventory.Contains(itemID))
            {
                mirrorPlayerInventory.Add(itemID);
            }
        }
    }
    public void CreateMirrorCustomSubPlayerInventory()
    {
        mirrorCustomSubPlayerInventory.Clear(); //Does this work?
        foreach (int itemID in mirrorPlayerInventory)
        {
            foreach (int allowedItemID in customPlayerSubInventoryAllowedItems)
            {
                bool canAddItem = false;
                if (itemID == allowedItemID)
                {
                    canAddItem = true;
                }
                if (canAddItem)
                {
                    mirrorCustomSubPlayerInventory.Add(itemID);
                }
            }
        }
    }
    public void AddItemToMirrorInventory(int itemID)
    {
        mirrorPlayerInventory.Add(itemID);
        foreach (int allowedItemID in customPlayerSubInventoryAllowedItems)
        {
            bool canAddItem = false;
            if (itemID == allowedItemID)
            {
                canAddItem = true;
            }
            if (canAddItem)
            {
                mirrorCustomSubPlayerInventory.Add(itemID);
            }
        }
        SortInventory(mirrorCustomSubPlayerInventory, mirrorInventorySortMode);
        SortInventory(mirrorPlayerInventory, mirrorInventorySortMode);
        onMirrorInventoryItemAdded?.Invoke(itemID);
    }
    public void RemoveItemFromMirrorPlayerInventory(int itemID)
    {
        mirrorPlayerInventory.Remove(itemID);
        if (mirrorCustomSubPlayerInventory.Contains(itemID))
        {
            mirrorCustomSubPlayerInventory.Remove(itemID);
        }
        onMirrorInventoryItemRemoved?.Invoke(itemID);
    }
    public virtual void AddInitialItems()
    {
        while(itemIDsInInventory.Count < desiredInventoryItems)
        {
            int itemToAdd = UnityEngine.Random.Range(0, GameItemDictionary.instance.gameItemNames.Count);
            int amountToAdd = UnityEngine.Random.Range(1, 8);
            AddItemToInventory(itemToAdd, amountToAdd);
        }
        //for(int i = 0; i <= desiredInventoryItems; i++)
        //{
            //int itemToAdd = UnityEngine.Random.Range(0, GameItemDictionary.instance.gameItemNames.Count);
            //int amountToAdd = UnityEngine.Random.Range(1, 8);
            //AddItemToInventory(itemToAdd, amountToAdd);
        //}
    }
    public virtual void ShuffleInventory()
    {
        int inventoryCountUpperBound = Mathf.FloorToInt(desiredInventoryItems * (1 + desiredInventoryItemsVariance));
        int inventoryCountLowerBound = Mathf.CeilToInt(desiredInventoryItems * (1 - desiredInventoryItemsVariance));
        int maxItemDelta = Mathf.CeilToInt(desiredInventoryItems * desiredInventoryItemsVariance);
        //ITEM DELTA SELECTION
        int itemDelta;
        if(itemIDsInInventory.Count > inventoryCountUpperBound)
        {
            itemDelta = UnityEngine.Random.Range(-maxItemDelta, 0);
        }else if(itemIDsInInventory.Count < inventoryCountLowerBound)
        {
            itemDelta = UnityEngine.Random.Range(0, maxItemDelta);
        }
        else
        {
            itemDelta = UnityEngine.Random.Range(-maxItemDelta, maxItemDelta);
        }
        //ITEM DELTA CORRECTION
        if((itemDelta + itemIDsInInventory.Count) > GameItemDictionary.instance.gameItemNames.Count)
        {
            itemDelta = GameItemDictionary.instance.gameItemNames.Count - itemIDsInInventory.Count;
        }
        if(itemIDsInInventory.Count + itemDelta < 0)
        {
            itemDelta = -itemIDsInInventory.Count;
        }
        //SHUFFLE
        int newInventoryCount = itemIDsInInventory.Count + itemDelta;
        if (canShuffleInventory)
        {
            while (itemIDsInInventory.Count < newInventoryCount)
            {
                //Debug.Log($"SHUFFLER IS ADDING ITEMS TO INVENTORY");
                int itemToAdd = UnityEngine.Random.Range(0, GameItemDictionary.instance.gameItemNames.Count);
                int amountToAdd = UnityEngine.Random.Range(1, 6);
                //Debug.Log($"Shuffler will add {amountToAdd} of " +
                //$"item ID {itemToAdd}: {GameItemDictionary.instance.gameItemNames[itemToAdd]}");
                AddItemToInventory(itemToAdd, amountToAdd);
            }
            while (itemIDsInInventory.Count > newInventoryCount)
            {
                //Debug.Log($"SHUFFLER IS REMOVING ITEMS FROM INVENTORY");
                int itemToRemove = itemIDsInInventory[UnityEngine.Random.Range(0, itemIDsInInventory.Count)];
                int amountToRemove = UnityEngine.Random.Range(1, itemAmount[itemToRemove] + 1);
                //Debug.Log($"Shuffler will remove {amountToRemove} of " +
                //$"item ID {itemToRemove}: {GameItemDictionary.instance.gameItemNames[itemToRemove]}");
                RemoveItemFromInventory(itemToRemove, amountToRemove);
            }
            if(storeCash >= 20000)
            {
                storeCash += UnityEngine.Random.Range(-3000, 501);
                OnInventoryCashChanged();
            }
            if(storeCash >= 5000 && storeCash < 20000)
            {
                storeCash += UnityEngine.Random.Range(-2000, 2001);
                OnInventoryCashChanged();
            }
            if (storeCash < 5000 && storeCash >= 1000)
            {
                storeCash += UnityEngine.Random.Range(-500, 3001);
                OnInventoryCashChanged();
            }
            if(storeCash < 1000)
            {
                storeCash += UnityEngine.Random.Range(2000, 5001);
                OnInventoryCashChanged();
            }
        }
        StartCoroutine(StartInventoryShuffleCounter(UnityEngine.Random.Range(180, 301))); //3(180) to 5(300) minutes.
        //StartCoroutine(StartInventoryShuffleCounter(1));
    }
    public void OnInventoryCashChanged()
    {
        onInventoryCashChanged?.Invoke();
    }
    //SORTING
    public void ChangeSortMode(string desiredSortMode) //Seperate mirror inventory from store inventory?
    {
        if (desiredSortMode == "Value")
        {
            sortMode = "Value";
            mirrorInventorySortMode = "Value";
            SortInventory(itemIDsInInventory, "Value");
            SortInventory(mirrorPlayerInventory, "Value");
            SortInventory(mirrorCustomSubPlayerInventory, "Value");
        }
        else if (desiredSortMode == "Name")
        {
            sortMode = "Name";
            mirrorInventorySortMode = "Name";
            SortInventory(itemIDsInInventory, "Name");
            SortInventory(mirrorPlayerInventory, "Name");
            SortInventory(mirrorCustomSubPlayerInventory, "Name");
        }
        else
        {
            Debug.LogWarning("Desired sort mode does not exist. Sorting by default.");
            sortMode = "Name";
            mirrorInventorySortMode = "Name";
            itemIDsInInventory.Sort();
            mirrorPlayerInventory.Sort();
            mirrorCustomSubPlayerInventory.Sort();
        }
        onSortModeChanged?.Invoke();
        onMirrorInventorySortModeChanged?.Invoke();
    }
    public void SortInventory(List<int> inventoryToSort, string desiredSortMode)
    {
        if (inventoryToSort.Count > 1)
        {
            if (desiredSortMode == "Value")
            {
                SortByValue(inventoryToSort);
            }
            if(desiredSortMode == "Name")
            {
                inventoryToSort.Sort();
            }
        }
        else
        {
            //Debug.LogWarning("Tried to sort inventory, but inventory does not have enough items to be sorted");
        }
    }
    public void SortByValue(List<int> inventoryToSort) //UPDATE THIS TO WORK WITH ANY INVENTORY
    {
        int buffer = 0;
        List<int> bufferInventory = new List<int>();
        bool sortComplete = false;
        while (!sortComplete)
        {
            buffer = 0;
            foreach (int itemID in inventoryToSort)
            {
                if (bufferInventory.Contains(itemID)) { continue; }
                if (bufferInventory.Contains(buffer) || !inventoryToSort.Contains(buffer))
                {
                    buffer = itemID;
                }
                if (GameItemDictionary.instance.gameItemValues[itemID] > GameItemDictionary.instance.gameItemValues[buffer])
                {
                    buffer = itemID;
                }
            }
            bufferInventory.Add(buffer);
            if (inventoryToSort.Count == bufferInventory.Count)
            {
                sortComplete = true;
            }
        }
        for (int i = 0; i < bufferInventory.Count; i++)
        {
            inventoryToSort[i] = bufferInventory[i];
        }
    }
    //COROUTINES
    public IEnumerator StartInventoryShuffleCounter(int timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        ShuffleInventory();
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
        int randomAmount = UnityEngine.Random.Range(1, 10);
        RemoveItemFromInventory(randomItemID, randomAmount);
        Debug.Log("Removed " + randomAmount + " " + GameItemDictionary.instance.gameItemNames[randomItemID] +
            "(s) from" + shopId + " inventory.");
    }
}
