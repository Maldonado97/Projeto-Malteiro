using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventoryScreenManager : MonoBehaviour
{
    public static PlayerInventoryScreenManager instance;
    [Header("Item UI")]
    [SerializeField] GameObject inventoryItemUI;
    [Tooltip("Where the inventoryItemUI should spawn.")]
    [SerializeField] GameObject inventoryItemUIParent;
    [Header("Inventory Information Area")]
    [SerializeField] TextMeshProUGUI playerCashTM;
    [SerializeField] TextMeshProUGUI carryCapacityTM;
    [Header("Item Description Area")]
    [SerializeField] TextMeshProUGUI itemDescriptionTM;
    [SerializeField] TextMeshProUGUI itemWeightTM;
    [SerializeField] TextMeshProUGUI itemValueTM;
    [SerializeField] GameObject useButton;
    [Header("TESTING")]
    [SerializeField] bool inTestingMode = false;

    [HideInInspector] public int selectedItemID;
    private string itemFilter = "All";
    public List<InventoryItemUI> itemUIs;
    protected List<int> activePlayerInventory = new List<int>();
    protected List<int> previousPlayerInventory = new List<int>();

    public event Action onAllItemsDeselected;
    //public event Action<int> onInventoryItemUIRemoved;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        selectedItemID = GameItemDictionary.instance.gameItemNames.Count;
        activePlayerInventory = PlayerInventoryManager.instance.itemIDsInInventory;
        UpdateCarryCapacityText();
        UpdatePlayerCashText();

        PlayerInventoryManager.instance.onInventoryItemAdded += OnInventoryItemAdded;
        PlayerInventoryManager.instance.onInventoryWeightChanged += UpdateCarryCapacityText;
        PlayerInventoryManager.instance.onInventoryCashChanged += UpdatePlayerCashText;

        PlayerInventoryManager.instance.onInventoryChanged += OnInventoryItemChanged;
        PlayerInventoryManager.instance.onInventoryItemRemoved += RemoveItemUI;
        PlayerInventoryManager.instance.onSortModeChanged += OnSortModeChanged;
    }
    private void Update()
    {
        if (inTestingMode)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (itemFilter == "All")
                {
                    ChangeInventoryFilter("Resource");
                }
                else if (itemFilter == "Resource")
                {
                    ChangeInventoryFilter("Cargo");
                }
                else
                {
                    ChangeInventoryFilter("All");
                }
            }
        }
    }
    public void OnInventoryItemAdded(int itemID)
    {
        if (activePlayerInventory.Contains(itemID))
        {
            CreateItemUI();
        }
    }
    public void CreateItemUI()
    {
        Instantiate(inventoryItemUI, inventoryItemUIParent.transform);
        FlashInventoryScreen();
    }
    public void SubscribeItemUI(InventoryItemUI newItemUI)
    {
        itemUIs.Add(newItemUI);
        foreach (InventoryItemUI itemUI in itemUIs)
        {
            itemUI.RefreshItemUI();
        }
    }
    public void GetItemUIItemID(InventoryItemUI itemUI)
    {
        //itemUI.myItemID = PlayerInventoryManager.instance.itemIDsInInventory[itemUI.orderInList];
        itemUI.myItemID = activePlayerInventory[itemUI.orderInList];
    }
    public void OnInventoryItemChanged(int itemID)
    {
        foreach (InventoryItemUI itemUI in itemUIs)
        {
            itemUI.OnInventoryChanged(itemID);
        }
    }
    public void RemoveItemUI(int itemID)
    {
        //Debug.Log("Removing ItemUI");
        bool removedItemFound = false;
        InventoryItemUI itemUIToRemove = null;
        foreach (InventoryItemUI itemUI in itemUIs)
        {
            if (itemUI.myItemID == itemID)
            {
                removedItemFound = true;
                Destroy(itemUI.gameObject);
                itemUIToRemove = itemUI;
            }
            if (removedItemFound)
            {
                itemUI.orderInList -= 1;
            }
        }
        itemUIs.Remove(itemUIToRemove);
    }
    public void OnSortModeChanged()
    {
        foreach (InventoryItemUI itemUI in itemUIs)
        {
            itemUI.RefreshItemUI();
            if (itemUI.myItemID == selectedItemID)
            {
                OnInventoryItemSelected(itemUI);
            }
        }
    }
    public void ChangeInventoryFilter(string newFilter)
    {
        if (newFilter == "All")
        {
            //Debug.Log("Switching to All items Filter");
            itemFilter = "All";
            previousPlayerInventory = activePlayerInventory;
            activePlayerInventory = PlayerInventoryManager.instance.itemIDsInInventory;
        }
        if (newFilter == "Cargo")
        {
            //Debug.Log("Switching to Fuel items Filter");
            itemFilter = "Cargo";
            previousPlayerInventory = activePlayerInventory;
            activePlayerInventory = PlayerInventoryManager.instance.cargo;
            //foreach (int itemID in PlayerInventoryManager.instance.cargo)
            //{
                //Debug.Log($"{GameItemDictionary.instance.gameItemNames[itemID]}: {PlayerInventoryManager.instance.itemAmount[itemID]}");
            //}
        }
        if(newFilter == "Resource")
        {
            itemFilter = "Resource";
            previousPlayerInventory = activePlayerInventory;
            activePlayerInventory = PlayerInventoryManager.instance.resources;
            //foreach (int itemID in PlayerInventoryManager.instance.resources)
            //{
            //Debug.Log($"{GameItemDictionary.instance.gameItemNames[itemID]}: {PlayerInventoryManager.instance.itemAmount[itemID]}");
            //}
        }

        if (activePlayerInventory.Count < itemUIs.Count)
        {
            while (activePlayerInventory.Count < itemUIs.Count)
            {
                //mirrorItemUIs.RemoveAt(mirrorItemUIs.Count - 1);
                //mirrorItemUIs.Remove(mirrorItemUIs[mirrorItemUIs.Count - 1]);
                RemoveItemUI(itemUIs[itemUIs.Count - 1].myItemID);
            }
        }
        if (activePlayerInventory.Count > itemUIs.Count)
        {
            while (activePlayerInventory.Count > itemUIs.Count)
            {
                CreateItemUI();
            }
        }
        ReloadInventoryItemUIList(itemUIs);
    }
    public void ReloadInventoryItemUIList(List<InventoryItemUI> itemUIList)
    {
        foreach (InventoryItemUI itemUI in itemUIList)
        {
            itemUI.RefreshItemUI();
        }
    }
    public void FlashInventoryScreen()
    {
        if (!UIManager.instance.playerMenu.activeSelf)
        {
            UIManager.instance.playerMenu.SetActive(true);
            UIManager.instance.playerMenu.SetActive(false);
        }
    }
    public void DisplayItemDescription(string itemDescription, float itemWeight, float itemValue)
    {
        itemDescriptionTM.text = itemDescription;
        itemWeightTM.text = "Weight: " + itemWeight.ToString() + "Kg";
        itemValueTM.text = "Value: $" + itemValue.ToString();
    }
    public void DeselectAllItems()
    {
        onAllItemsDeselected?.Invoke();
        itemDescriptionTM.text = "-";
        itemWeightTM.text = "Weight: -";
        itemValueTM.text = "Value: -";
        selectedItemID = GameItemDictionary.instance.gameItemNames.Count + 1;
    }
    public void UpdateCarryCapacityText()
    {
        carryCapacityTM.text = $"Carry Capacity: {PlayerInventoryManager.instance.totalWeight}/{PlayerInventoryManager.instance.maxWeight}";
    }
    public void UpdatePlayerCashText()
    {
        playerCashTM.text = $"Cash: {PlayerInventoryManager.instance.playerCash}";
    }
    //EVENT METHODS
    public void OnInventoryItemSelected(InventoryItemUI inventoryItem)
    {
        var inventoryItemUIScript = inventoryItem.GetComponent<InventoryItemUI>();
        var GID = GameItemDictionary.instance;

        DeselectAllItems();
        inventoryItemUIScript.SelectThisItem();
        selectedItemID = inventoryItemUIScript.myItemID;
        DisplayItemDescription(GID.gameItemDescriptions[selectedItemID], GID.gameItemWeights[selectedItemID],
            GID.gameItemValues[selectedItemID]);
    }
    public void OnInventoryItemUIRemoved(int removedItemUIOrderInList)
    {
        //onInventoryItemUIRemoved?.Invoke(removedItemUIOrderInList);
    }
}
