using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class GeneralDockShopScreenManager : MonoBehaviour
{
    [Header("Dock UI Manager")]
    [SerializeField] DockUIManager dockUIManager;
    [Header("Item UIs")]
    [SerializeField] GameObject inventoryItemUI;
    [SerializeField] GameObject mirrorPlayerInventoryItemUI;
    [Header("Item UI Parents")]
    [Tooltip("The store's screen.")]
    public GameObject storeScreen;
    [Tooltip("Where own inventoryItemUI should spawn.")]
    [SerializeField] GameObject ownInventoryPanel;
    [Tooltip("Where the mirror player inventory item UI should spawn.")]
    [SerializeField] GameObject mirrorPlayerInventoryPanel;
    [Header("Player Inventory Information")]
    [SerializeField] TextMeshProUGUI playerCashTM;
    [SerializeField] TextMeshProUGUI playerCarryCapacityTM;
    [Header("Store Information")]
    [SerializeField] TextMeshProUGUI storeCashTM;
    [Header("TESTING")]
    public bool inTestingMode = false;
    public List<DockShopInventoryItemUI> itemUIs;
    public List<DockShopInventoryItemUI> mirrorItemUIs;
    protected GeneralDockInventoryManager ownInventory;
    private string itemTypeToDisplay = "All";
    protected List<int> activePlayerInventory = new List<int>();
    protected List<int> previousPlayerInventory = new List<int>();

    //public event Action<int> onInventoryItemUIRemoved;
    //public event Action<GeneralDockShopScreenManager> onIventoryItemUICreated;
    //public event Action<int> onMirrorPlayerInventoryItemUIRemoved;
    public event Action<int> onItemTransferConfirmed;
    public event Action onItemTransferCanceled;

    public virtual void Start()
    {
        SetInstance();
        SetOwnInventoryReference();
        SubscribeToEvents();
        activePlayerInventory = PlayerInventoryManager.instance.itemIDsInInventory;
        UpdatePlayerCarryCapacityText();
        UpdatePlayerCashText();
        UpdateStoreCashText();
    }
    public void Update()
    {
        if (storeScreen.activeSelf)
        {
            ownInventory.canShuffleInventory = false;
        }
        else
        {
            ownInventory.canShuffleInventory = true;
        }
        if (inTestingMode)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if(itemTypeToDisplay == "All")
                {
                    ChangePlayerInventoryFilter("Fuel");
                }
                else
                {
                    ChangePlayerInventoryFilter("All");
                }
            }
        }
    }
    public abstract void SetInstance();
    public abstract void SetOwnInventoryReference();
    public virtual void SubscribeToEvents()
    {
        PlayerInventoryManager.instance.onInventoryChanged += OnPlayerInventoryItemChanged;
        PlayerInventoryManager.instance.onInventoryItemAdded += CreateMirrorPlayerInventoryItemUI;
        PlayerInventoryManager.instance.onInventoryItemRemoved += RemoveMirrorItemUI;
        PlayerInventoryManager.instance.onSortModeChanged += OnPlayerInventorySortModeChanged;
        PlayerInventoryManager.instance.onInventoryWeightChanged += UpdatePlayerCarryCapacityText;
        PlayerInventoryManager.instance.onInventoryCashChanged += UpdatePlayerCashText;
        ownInventory.onInventoryItemAdded += CreateItemUI;
        ownInventory.onInventoryItemRemoved += RemoveItemUI;
        ownInventory.onSortModeChanged += OnOwnInventorySortModeChanged;
        ownInventory.onInventoryChanged += OnOwnInventoryItemChanged;
        ownInventory.onInventoryCashChanged += UpdateStoreCashText;
    }
    //OWN INVENTORY
    public void CreateItemUI()
    {
        Instantiate(inventoryItemUI, ownInventoryPanel.transform);
        dockUIManager.FlashStoreScreen(this);
    }
    public void SubscribeItemUI(DockShopInventoryItemUI newItemUI)
    {
        itemUIs.Add(newItemUI);
        foreach (DockShopInventoryItemUI itemUI in itemUIs)
        {
            itemUI.RefreshItemUI();
        }
    }
    public void GetItemUIItemID(DockShopInventoryItemUI itemUI)
    {
        itemUI.myItemID = ownInventory.itemIDsInInventory[itemUI.orderInList];
    }
    public void OnOwnInventoryItemChanged(int itemID)
    {
        foreach(DockShopInventoryItemUI itemUI in itemUIs)
        {
            itemUI.OnInventoryChanged(itemID);
        }
    }
    public void RemoveItemUI(int itemID)
    {
        //Debug.Log("Removing ItemUI");
        bool removedItemFound = false;
        DockShopInventoryItemUI itemUIToRemove = null;
        foreach(DockShopInventoryItemUI itemUI in itemUIs)
        {
            if(itemUI.myItemID == itemID)
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
    public void OnOwnInventorySortModeChanged()
    {
        foreach (DockShopInventoryItemUI itemUI in itemUIs)
        {
            itemUI.RefreshItemUI();
        }
    }
    //MIRROR PLAYER INVENTORY
    public void CreateMirrorPlayerInventoryItemUI()
    {
        if (activePlayerInventory.Count > mirrorItemUIs.Count) //Checks if item added is part of the active inventory
        {
            Instantiate(mirrorPlayerInventoryItemUI, mirrorPlayerInventoryPanel.transform);
            dockUIManager.FlashStoreScreen(this);
        }
    }
    public void SubscribeMirrorItemUI(DockShopInventoryItemUI newItemUI)
    {
        mirrorItemUIs.Add(newItemUI);
        foreach (DockShopInventoryItemUI mirrorItemUI in mirrorItemUIs)
        {
            mirrorItemUI.RefreshItemUI();
        }
    }
    public void GetMirrorItemUIItemID(DockShopInventoryItemUI itemUI)
    {
        itemUI.myItemID = activePlayerInventory[itemUI.orderInList];
    }
    public void OnPlayerInventoryItemChanged(int itemID)
    {
        foreach (DockShopInventoryItemUI mirrorItemUI in mirrorItemUIs)
        {
            mirrorItemUI.OnInventoryChanged(itemID);
        }
    }
    public void RemoveMirrorItemUI(int itemID)
    {
        bool removedItemFound = false;
        DockShopInventoryItemUI itemUIToRemove = null;
        //Debug.Log("Entering Foreach loop");
        foreach (DockShopInventoryItemUI itemUI in mirrorItemUIs)
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
        mirrorItemUIs.Remove(itemUIToRemove);
    }
    public void OnPlayerInventorySortModeChanged()
    {
        foreach (DockShopInventoryItemUI mirrorItemUI in mirrorItemUIs)
        {
            mirrorItemUI.RefreshItemUI();
        }
    }
    public void ChangePlayerInventoryFilter(string newFilter)
    {
        if(newFilter == "All")
        {
            Debug.Log("Switching to All items Filter");
            itemTypeToDisplay = "All";
            previousPlayerInventory = activePlayerInventory;
            activePlayerInventory = PlayerInventoryManager.instance.itemIDsInInventory;
        }
        if (newFilter == "Fuel")
        {
            Debug.Log("Switching to Fuel items Filter");
            itemTypeToDisplay = "Fuel";
            previousPlayerInventory = activePlayerInventory;
            activePlayerInventory = PlayerInventoryManager.instance.fuelItemsInInventory;
            foreach(int itemID in PlayerInventoryManager.instance.fuelItemsInInventory)
            {
                Debug.Log($"{GameItemDictionary.instance.gameItemNames[itemID]}: {PlayerInventoryManager.instance.itemAmount[itemID]}");
            }
        }
        if (activePlayerInventory.Count < mirrorItemUIs.Count)
        {
            while (activePlayerInventory.Count < mirrorItemUIs.Count)
            {
                //mirrorItemUIs.RemoveAt(mirrorItemUIs.Count - 1);
                //mirrorItemUIs.Remove(mirrorItemUIs[mirrorItemUIs.Count - 1]);
                RemoveMirrorItemUI(mirrorItemUIs[mirrorItemUIs.Count - 1].myItemID);
            }
        }
        if(activePlayerInventory.Count > mirrorItemUIs.Count)
        {
            while (activePlayerInventory.Count > mirrorItemUIs.Count)
            {
                CreateMirrorPlayerInventoryItemUI();
            }
        }
        ReloadInventoryItemUIList(mirrorItemUIs);
    }
    public void ReloadInventoryItemUIList(List<DockShopInventoryItemUI> itemUIList)
    {
        foreach (DockShopInventoryItemUI itemUI in itemUIList)
        {
            itemUI.RefreshItemUI();
        }
    }
    //INVENTORY INFORMATION
    public void UpdatePlayerCarryCapacityText()
    {
        playerCarryCapacityTM.text = $"Carry Capacity: {PlayerInventoryManager.instance.totalWeight}/{PlayerInventoryManager.instance.maxWeight}";
    }
    public void UpdatePlayerCashText()
    {
        playerCashTM.text = $"Cash: {Mathf.RoundToInt(PlayerInventoryManager.instance.playerCash)}";
    }
    public void UpdateStoreCashText()
    {
        storeCashTM.text = $"Store Cash: {Mathf.RoundToInt(ownInventory.storeCash)}";
    }
    //TRANSFER AMOUNT SELECTOR
    public void OpenTransferAmountSelector(int itemAmount, float itemValue)
    {
        //this has to be here because ItemUIs need this method, but they can only access it through the shop screen
        //manager.
        dockUIManager.OpenTransferAmountSelector(itemAmount, itemValue, this);
    }
    public void OnItemTransferConfirmed()
    {
        onItemTransferConfirmed?.Invoke(Mathf.RoundToInt(dockUIManager.selectorSlider.value));
    }
    public void OnItemTransferCanceled()
    {
        onItemTransferCanceled?.Invoke();
    }
    public void OpenInsufficientPlayerFundsWarning()
    {
        dockUIManager.OpenInsufficientPlayerFundsWarning();
    }
    public void OpenInsufficientStoreFundsWarning()
    {
        dockUIManager.OpenInsufficientStoreFundsWarning();
    }
}
