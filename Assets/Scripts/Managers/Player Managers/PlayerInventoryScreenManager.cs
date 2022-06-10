using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventoryScreenManager : MonoBehaviour
{
    public static PlayerInventoryScreenManager instance;
    [Header("Inventory Tabs")]
    [SerializeField] List<BoxButton> inventoryTabs = new List<BoxButton>();
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
    [Header("Item Use")]
    [SerializeField] GameObject fuelFullWarning;
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
        DeselectAllItems();
        //selectedItemID = GameItemDictionary.instance.gameItemNames.Count;
        activePlayerInventory = PlayerInventoryManager.instance.itemIDsInInventory;
        UpdateCarryCapacityText();
        UpdatePlayerCashText();
        DisplayCargoSubInventory();

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
                if (itemFilter == "Resource")
                {
                    DisplayCargoSubInventory();
                }
                else
                {
                    DisplayResourcesSubInventory();
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
    public void DeselectAllItems()
    {
        onAllItemsDeselected?.Invoke();
        itemDescriptionTM.text = "-";
        itemWeightTM.text = ": -";
        itemValueTM.text = ": -";
        selectedItemID = GameItemDictionary.instance.gameItemNames.Count + 1;
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
    //DISPLAY
    public void DisplayItemDescription(string itemDescription, float itemWeight, float itemValue)
    {
        itemDescriptionTM.text = itemDescription;
        itemWeightTM.text = ": " + itemWeight.ToString() + "Kg";
        itemValueTM.text = ": $" + itemValue.ToString();
    }
    public void UpdateCarryCapacityText()
    {
        carryCapacityTM.text = $": {PlayerInventoryManager.instance.totalWeight}/{PlayerInventoryManager.instance.maxWeight}";
    }
    public void UpdatePlayerCashText()
    {
        playerCashTM.text = $": {PlayerInventoryManager.instance.playerCash}";
    }
    public void DisplayCargoSubInventory()
    {
        useButton.SetActive(false);
        ChangeInventoryFilter("Cargo");
        foreach(BoxButton inventoryTab in inventoryTabs)
        {
            if(inventoryTab.gameObject.name == "Cargo Tab")
            {
                SelectInventoryTab(inventoryTab);
            }
        }
    }
    public void DisplayResourcesSubInventory()
    {
        useButton.SetActive(true);
        ChangeInventoryFilter("Resource");
        foreach (BoxButton inventoryTab in inventoryTabs)
        {
            if (inventoryTab.gameObject.name == "Resource Tab")
            {
                SelectInventoryTab(inventoryTab);
            }
        }
    }
    //INVENTORY TABS
    public void DeselectAllInventoryTabs()
    {
        foreach(BoxButton inventoryTab in inventoryTabs)
        {
            //REPLACE FOR inventoryTab.DeselectButton();
            inventoryTab.selected = false;
            inventoryTab.buttonTextTMPro.color = inventoryTab.defaultTextColor;
            inventoryTab.boxImage.color = inventoryTab.defaultBoxColor;
        }
    }
    public void SelectInventoryTab(BoxButton inventoryTab)
    {
        DeselectAllInventoryTabs();
        inventoryTab.selected = true;
        //Replace these for "inventoryTab.SelectButton();"
        inventoryTab.buttonTextTMPro.color = inventoryTab.selectedTextColor;
        inventoryTab.boxImage.color = inventoryTab.selectedBoxColor;
    }
    //PLAYER INTERACTION
    public void UseSelectedItem()
    {
        var inventoryManager = PlayerInventoryManager.instance;
        float fuelDrumCapacity = 200;
        if (selectedItemID == 4) //Fuel Drum
        {
            //Debug.Log($"Fuel: {inventoryManager.fuel}, Max Fuel: {inventoryManager.maxFuel}," +
               // $" Fuel Drum Capacity: {fuelDrumCapacity}");
            if(inventoryManager.fuel <= (inventoryManager.maxFuel - fuelDrumCapacity))
            {
                inventoryManager.RemoveItemFromInventory(4, 1);
                inventoryManager.fuel += fuelDrumCapacity;
            }
            else
            {
                fuelFullWarning.SetActive(true);
                //Debug.Log("Replenishing fuel now will overflow tank");
            }
        }
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
