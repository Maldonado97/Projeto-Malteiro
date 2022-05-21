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
    //[Header("Item Transfer Amount Selector")] REMOVE ALL THIS
    //[SerializeField] GameObject transferAmountSelector;
    //[SerializeField] GameObject selectorSliderTM;
    //[SerializeField] GameObject selectedAmountTM;
    //[SerializeField] TextMeshProUGUI transactionValueTM;
    //private float transferingItemValue;

    protected GeneralDockInventoryManager ownInventory;
    //private Slider selectorSlider; REMOVE THIS
    //private TextMeshProUGUI selectorText; REMOVE THIS
     
    public event Action<int> onInventoryItemUIRemoved;
    //public event Action<GeneralDockShopScreenManager> onIventoryItemUICreated;
    public event Action<int> onMirrorPlayerInventoryItemUIRemoved;
    public event Action<int> onItemTransferConfirmed;
    public event Action onItemTransferCanceled;

    public void Start()
    {
        SetInstance();
        SetOwnInventoryReference();
        SubscribeToEvents();
        //GetTransferAmountSelectorComponents(); REMOVE THIS

        UpdatePlayerCarryCapacityText();
        UpdatePlayerCashText();
        UpdateStoreCashText();

        //CloseTransferAmountSelector();
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
    }
    public abstract void SetInstance();
    public abstract void SetOwnInventoryReference();
    public virtual void SubscribeToEvents()
    {
        PlayerInventoryManager.instance.onInventoryItemAdded += CreateMirrorPlayerInventoryItemUI;
        PlayerInventoryManager.instance.onInventoryWeightChanged += UpdatePlayerCarryCapacityText;
        PlayerInventoryManager.instance.onInventoryCashChanged += UpdatePlayerCashText;
        ownInventory.onInventoryCashChanged += UpdateStoreCashText;
    }
    public void GetTransferAmountSelectorComponents() //REMOVE THIS
    {
        //selectorSlider = selectorSliderTM.GetComponent<Slider>();
        //selectorText = selectedAmountTM.GetComponent<TextMeshProUGUI>();
    }
    public void CreateItemUI()
    {
        Instantiate(inventoryItemUI, ownInventoryPanel.transform);
        OnInventoryItemUICreated();
        //onIventoryItemUICreated?.Invoke(this);
    }
    public void CreateMirrorPlayerInventoryItemUI()
    {
        Instantiate(mirrorPlayerInventoryItemUI, mirrorPlayerInventoryPanel.transform);
        OnMirrorInventoryItemUICreated();
        //onIventoryItemUICreated?.Invoke(this); REMOVE THIS
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
    //EVENT METHODS
    public void OnInventoryItemUICreated()
    {
        dockUIManager.FlashStoreScreen(this);
    }
    public void OnMirrorInventoryItemUICreated()
    {
        dockUIManager.FlashStoreScreen(this);
    }
    public void OnInventoryItemUIRemoved(int removedItemUIOrderInList)
    {
        onInventoryItemUIRemoved?.Invoke(removedItemUIOrderInList);
    }
    public void OnMirrorPlayerInventoryItemUIRemoved(int removedItemUIOrderInList)
    {
        onMirrorPlayerInventoryItemUIRemoved?.Invoke(removedItemUIOrderInList);
    }
    //TRANSFER AMOUNT SELECTOR
    public void OpenTransferAmountSelector(int itemAmount, float itemValue)
    {
        //selectorSlider.maxValue = itemAmount;
        //transferingItemValue = itemValue;
        //UpdateTransferAmountSelectorText();
        //transferAmountSelector.SetActive(true);

        //this has to be here because ItemUIs need this method, but they can only access it through the shop screen
        //manager.
        dockUIManager.OpenTransferAmountSelector(itemAmount, itemValue, this);
    }
    //public void CloseTransferAmountSelector() REMOVE THIS
    //{
        //transferAmountSelector.SetActive(false);
        //this is here just for consistancy. The Open method is here, so why not leave the close method.
        //dockUIManager.CloseTransferAmountSelector();
    //}
    //public void UpdateTransferAmountSelectorText() //REMOVE THIS
    //{
        //selectorText.text = ("Choose Amount: " + selectorSlider.value);
        //transactionValueTM.text = ($"Value: {Mathf.RoundToInt(selectorSlider.value * transferingItemValue)}");
    //}
    public void OnItemTransferConfirmed()
    {
        onItemTransferConfirmed?.Invoke(Mathf.RoundToInt(dockUIManager.selectorSlider.value));
        //dockUIManager.selectorSlider.value = 1;
        //CloseTransferAmountSelector();
    }
    public void OnItemTransferCanceled()
    {
        onItemTransferCanceled?.Invoke();
        //dockUIManager.selectorSlider.value = 1;
        //CloseTransferAmountSelector();
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
