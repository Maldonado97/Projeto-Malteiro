using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public abstract class DockShopInventoryItemUI : CustomButton
{
    [HideInInspector] public GeneralDockShopScreenManager shopScreenManager;
    [HideInInspector] public GeneralDockInventoryManager shopInventoryManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public GameItemDictionary gameItemDictionary;
    [Header("UI Elements")]
    [SerializeField] protected Color highlightedCellColor;
    [SerializeField] protected Color defaultCellColor;
    [SerializeField] protected TextMeshProUGUI itemNameTM;
    [SerializeField] protected TextMeshProUGUI itemAmountTM;
    [SerializeField] protected TextMeshProUGUI itemValueTM;
    [SerializeField] protected TextMeshProUGUI itemWeightTM;

    protected Image cell;
    protected int orderInList;
    [HideInInspector] public int myItemID;
    [HideInInspector] public string myItemName;
    [HideInInspector] public int myItemAmount;
    [HideInInspector] public float myItemValue;
    [HideInInspector] public float myItemWeight;
    
    protected bool transferingItem = false;

    protected void Start()
    {
        SetParentSingletonReferences();
        playerInventoryManager = PlayerInventoryManager.instance;
        gameItemDictionary = GameItemDictionary.instance;

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        SubscribeToEvents();
        GetItemInformation();
        SetUIInformation();
    }
    protected abstract void SetParentSingletonReferences();
    protected virtual void SubscribeToEvents()
    {
        shopInventoryManager.onInventoryChanged += OnInventoryChanged;
        shopInventoryManager.onInventoryItemAdded += OnInventoryItemAdded;
        shopInventoryManager.onInventoryItemRemoved += OnInventoryItemRemoved;
        shopInventoryManager.onSortModeChanged += OnSortModeChanged;
        shopScreenManager.onInventoryItemUIRemoved += OnInventoryItemUIRemoved;
        shopScreenManager.onItemTransferConfirmed += TransferMultipleItems;
        shopScreenManager.onItemTransferCanceled += CancelItemTransfer;
    }
    public void OnInventoryChanged(int changedItemID)
    {
        if (changedItemID == myItemID)
        {
            GetItemInformation();
            SetUIInformation();
        }
    }
    public void OnInventoryItemAdded()
    {
        GetItemInformation();
        SetUIInformation();
    }
    public void OnInventoryItemRemoved(int removedItemID)
    {
        if (removedItemID == myItemID)
        {
            Destroy(gameObject);
        }
    }
    public void OnInventoryItemUIRemoved(int removedItemUIOrderInList)
    {
        if (removedItemUIOrderInList < orderInList)
        {
            orderInList -= 1;
        }
    }
    public void OnSortModeChanged()
    {
        GetItemInformation();
        SetUIInformation();
    }
    public virtual void GetItemInformation()
    {
        myItemID = shopInventoryManager.itemIDsInInventory[orderInList];
        myItemAmount = shopInventoryManager.itemAmount[myItemID];
        myItemName = gameItemDictionary.gameItemNames[myItemID];
        myItemValue = gameItemDictionary.gameItemValues[myItemID];
        myItemWeight = gameItemDictionary.gameItemWeights[myItemID];
    }
    public void SetUIInformation()
    {
        itemNameTM.text = myItemName;
        itemAmountTM.text = myItemAmount.ToString();
        itemValueTM.text = myItemValue.ToString();
        itemWeightTM.text = myItemWeight.ToString();
    }
    public void HighlightCell()
    {
        cell.color = highlightedCellColor;
    }
    public void ResetCell()
    {
        cell.color = defaultCellColor;
    }
    public virtual void TransferSingleItem()
    {
        shopInventoryManager.RemoveItemFromInventory(myItemID, 1);
        playerInventoryManager.AddItemToInventory(myItemID, 1);
    }
    public virtual void TransferMultipleItems(int amountToTransfer)
    {
        if(transferingItem == true)
        {
            shopInventoryManager.RemoveItemFromInventory(myItemID, amountToTransfer);
            playerInventoryManager.AddItemToInventory(myItemID, amountToTransfer);
            transferingItem = false;
        }
    }
    public void CancelItemTransfer()
    {
        transferingItem = false;
    }
    protected virtual void OnDestroy()
    {
        shopScreenManager.OnInventoryItemUIRemoved(orderInList);
        UnsubscribeFromAllEvents();
    }
    protected virtual void UnsubscribeFromAllEvents()
    {
        shopInventoryManager.onInventoryChanged -= OnInventoryChanged;
        shopInventoryManager.onInventoryItemAdded -= OnInventoryItemAdded;
        shopInventoryManager.onInventoryItemRemoved -= OnInventoryItemRemoved;
        shopInventoryManager.onSortModeChanged -= OnSortModeChanged;
        shopScreenManager.onInventoryItemUIRemoved -= OnInventoryItemUIRemoved;
        shopScreenManager.onItemTransferConfirmed += TransferMultipleItems;
        shopScreenManager.onItemTransferCanceled += CancelItemTransfer;
    }

    //USER INTERFACE METHODS
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if(myItemAmount > 1)
        {
            transferingItem = true;
            shopScreenManager.OpenTransferAmountSelector(myItemAmount);
        }
        else
        {
            TransferSingleItem();
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        HighlightCell();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        ResetCell();
    }
}
