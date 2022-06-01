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
    private string itemFilter = "All";
    protected List<int> activePlayerInventory = new List<int>();
    protected List<int> previousPlayerInventory = new List<int>();
    //protected List<int> customPlayerSubInventory = new List<int>();
    //protected List<string> customPlayerSubInventoryItemTypes = new List<string>();

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
        //activePlayerInventory = PlayerInventoryManager.instance.itemIDsInInventory;
        activePlayerInventory = ownInventory.mirrorPlayerInventory;
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
                if(itemFilter == "All")
                {
                    ChangePlayerInventoryFilter("Custom");
                }else
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
        //PLAYER INVENTORY ITEM UI INFORMATION
        PlayerInventoryManager.instance.onInventoryChanged += OnPlayerInventoryItemChanged;
        PlayerInventoryManager.instance.onInventoryWeightChanged += UpdatePlayerCarryCapacityText;
        PlayerInventoryManager.instance.onInventoryCashChanged += UpdatePlayerCashText;
        //PlayerInventoryManager.instance.onInventoryItemAdded += OnPlayerInventoryItemAdded;
        //PlayerInventoryManager.instance.onInventoryItemRemoved += RemoveMirrorItemUI;
        PlayerInventoryManager.instance.onSortModeChanged += OnPlayerInventorySortModeChanged; //SOON TO BE REMOVED
        //OWN INVENTORY
        ownInventory.onInventoryItemAdded += CreateItemUI;
        ownInventory.onInventoryItemRemoved += RemoveItemUI;
        ownInventory.onSortModeChanged += OnOwnInventorySortModeChanged;
        ownInventory.onInventoryChanged += OnOwnInventoryItemChanged;
        ownInventory.onInventoryCashChanged += UpdateStoreCashText;
        //MIRROR INVENTORY
        ownInventory.onMirrorInventoryItemAdded += OnMirrorPlayerInventoryItemAdded;
        ownInventory.onMirrorInventoryItemRemoved += RemoveMirrorItemUI;
        ownInventory.onMirrorInventorySortModeChanged += OnPlayerInventorySortModeChanged;
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
        //if(itemFilter == "Custom") //Clean this code up later
        //{
            //OnPlayerInventorySortModeChanged();
        //}
        foreach (DockShopInventoryItemUI itemUI in itemUIs)
        {
            itemUI.RefreshItemUI();
        }
    }
    //MIRROR PLAYER INVENTORY
    public void ChangePlayerInventoryFilter(string newFilter)
    {
        if (newFilter == "All")
        {
            Debug.Log("Switching to All items Filter");
            itemFilter = "All";
            previousPlayerInventory = activePlayerInventory;
            activePlayerInventory = ownInventory.mirrorPlayerInventory;
        }
        //if (newFilter == "Fuel")
        //{
           //Debug.Log("Switching to Fuel items Filter");
            //itemFilter = "Fuel";
            //previousPlayerInventory = activePlayerInventory;
            //activePlayerInventory = PlayerInventoryManager.instance.fuelItemsInInventory; //MAKE LOCAL VERSION OF THIS
            //foreach (int itemID in PlayerInventoryManager.instance.fuelItemsInInventory)
            //{
                //Debug.Log($"{GameItemDictionary.instance.gameItemNames[itemID]}: {PlayerInventoryManager.instance.itemAmount[itemID]}");
            //}
        //}
        if (newFilter == "Custom")
        {
            Debug.Log("Switching to custom Filter");
            itemFilter = "Custom";
            previousPlayerInventory = activePlayerInventory;
            //UpdateCustomPlayerSubInventory();
            activePlayerInventory = ownInventory.mirrorCustomSubPlayerInventory;
            foreach (int itemID in ownInventory.mirrorCustomSubPlayerInventory)
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
        if (activePlayerInventory.Count > mirrorItemUIs.Count)
        {
            while (activePlayerInventory.Count > mirrorItemUIs.Count)
            {
                CreateMirrorPlayerInventoryItemUI();
            }
        }
        ReloadInventoryItemUIList(mirrorItemUIs);
    }
    public void OnMirrorPlayerInventoryItemAdded(int itemID)
    {
        //if(itemFilter == "Custom")
        //{
            //UpdateCustomPlayerSubInventory();
        //}
        if (activePlayerInventory.Contains(itemID)) //USELESS
        {
            CreateMirrorPlayerInventoryItemUI();
        }
    }
    public void CreateMirrorPlayerInventoryItemUI()
    {
        Instantiate(mirrorPlayerInventoryItemUI, mirrorPlayerInventoryPanel.transform);
        dockUIManager.FlashStoreScreen(this);
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
    //ITEM UI RELOAD
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
