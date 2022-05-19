using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class GeneralDockShopScreenManager : MonoBehaviour
{
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
    [Header("Item Transfer Amount Selector")]
    [SerializeField] GameObject transferAmountSelector;
    [SerializeField] GameObject selectorSliderTM;
    [SerializeField] GameObject selectedAmountTM;
    [SerializeField] TextMeshProUGUI transactionValueTM;

    protected GeneralDockInventoryManager ownInventory;
    private Slider selectorSlider;
    private TextMeshProUGUI selectorText;
    private float transferingItemValue;

    public event Action<int> onInventoryItemUIRemoved;
    public event Action<GeneralDockShopScreenManager> onIventoryItemUICreated;
    public event Action<int> onMirrorPlayerInventoryItemUIRemoved;
    public event Action<int> onItemTransferConfirmed;
    public event Action onItemTransferCanceled;

    public void Start()
    {
        SetInstance();
        SetOwnInventoryReference();
        SubscribeToEvents();
        GetTransferAmountSelectorComponents();

        UpdatePlayerCarryCapacityText();
        UpdatePlayerCashText();
        UpdateStoreCashText();

        CloseTransferAmountSelector();
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
    public void GetTransferAmountSelectorComponents()
    {
        selectorSlider = selectorSliderTM.GetComponent<Slider>();
        selectorText = selectedAmountTM.GetComponent<TextMeshProUGUI>();
    }
    public void CreateItemUI()
    {
        Instantiate(inventoryItemUI, ownInventoryPanel.transform);
        onIventoryItemUICreated?.Invoke(this);
    }
    public void CreateMirrorPlayerInventoryItemUI()
    {
        Instantiate(mirrorPlayerInventoryItemUI, mirrorPlayerInventoryPanel.transform);
        onIventoryItemUICreated?.Invoke(this);
    }
    //INVENTORY INFORMATION
    public void UpdatePlayerCarryCapacityText()
    {
        playerCarryCapacityTM.text = $"Carry Capacity: {PlayerInventoryManager.instance.totalWeight}/{PlayerInventoryManager.instance.maxWeight}";
    }
    public void UpdatePlayerCashText()
    {
        playerCashTM.text = $"Cash: {PlayerInventoryManager.instance.playerCash}";
    }
    public void UpdateStoreCashText()
    {
        storeCashTM.text = $"Store Cash: {ownInventory.storeCash}";
    }
    //EVENT METHODS
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
        selectorSlider.maxValue = itemAmount;
        transferingItemValue = itemValue;
        UpdateTransferAmountSelectorText();
        transferAmountSelector.SetActive(true);
    }
    public void CloseTransferAmountSelector()
    {
        transferAmountSelector.SetActive(false);
    }
    public void UpdateTransferAmountSelectorText()
    {
        selectorText.text = ("Choose Amount: " + selectorSlider.value);
        transactionValueTM.text = ($"Value: {Mathf.RoundToInt(selectorSlider.value * transferingItemValue)}");
    }
    public void ConfirmItemTransfer()
    {
        onItemTransferConfirmed?.Invoke(Mathf.RoundToInt(selectorSlider.value));
        selectorSlider.value = 1;
        CloseTransferAmountSelector();
    }
    public void CancelItemTransfer()
    {
        onItemTransferCanceled?.Invoke();
        selectorSlider.value = 1;
        CloseTransferAmountSelector();
    }
}
