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
    [HideInInspector] public float myModifiedItemValue;
    [HideInInspector] public float myItemWeight;

    //protected List<float> itemValueModifications = new List<float>();
    protected Dictionary<string, float> itemValueModifications = new Dictionary<string, float>();
    protected float baseValueModification;
    
    protected bool transferingItem = false;

    protected void Awake()
    {
        SetParentSingletonReferences();
        playerInventoryManager = PlayerInventoryManager.instance;
        gameItemDictionary = GameItemDictionary.instance;

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();
        //Debug.Log($"Adding {gameItemDictionary.gameItemNames[myItemID]} item UI.");

        SubscribeToEvents();
        SetItemValueModifications();
        GetItemID();
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
    protected abstract void SetItemValueModifications();
    public virtual void GetItemID() //Leave this separate please...
    {
        myItemID = shopInventoryManager.itemIDsInInventory[orderInList];
    }
    public virtual void GetItemInformation()
    {
        myItemAmount = shopInventoryManager.itemAmount[myItemID];
        myItemName = gameItemDictionary.gameItemNames[myItemID];
        myItemValue = gameItemDictionary.gameItemValues[myItemID];
        myItemWeight = gameItemDictionary.gameItemWeights[myItemID];
        //MODIFIED ITEM VALUE
        myModifiedItemValue = myItemValue + myItemValue * (baseValueModification + itemValueModifications[myItemName]);
    }
    public void SetUIInformation()
    {
        itemNameTM.text = myItemName;
        itemAmountTM.text = $"{myItemAmount}";
        itemValueTM.text = $"{Mathf.RoundToInt(myModifiedItemValue)}";
        itemWeightTM.text = $"{myItemWeight}";
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
        if (playerInventoryManager.playerCash >= myModifiedItemValue)
        {
            //ITEM TRANSFER
            shopInventoryManager.RemoveItemFromInventory(myItemID, 1);
            playerInventoryManager.AddItemToInventory(myItemID, 1);
            //CASH TRANSFER
            shopInventoryManager.storeCash += myModifiedItemValue;
            playerInventoryManager.playerCash -= myModifiedItemValue;
            //EVEN TRIGGER
            playerInventoryManager.OnInventoryCashChanged();
            shopInventoryManager.OnInventoryCashChanged();
        }
        else
        {
            shopScreenManager.OpenInsufficientPlayerFundsWarning();
            Debug.LogWarning($"You don't have enough cash to complete this transaction!");
        }
    }
    public virtual void TransferMultipleItems(int amountToTransfer)
    {
        //All itemUIs are subscribed to onItemTransferConfirmed. This first IF avoids having them all send their items
        //to the player inventory at once.
        if (transferingItem == true)
        {
            if(playerInventoryManager.playerCash >= myModifiedItemValue * amountToTransfer)
            {
                //ITEM TRANSFER
                shopInventoryManager.RemoveItemFromInventory(myItemID, amountToTransfer);
                playerInventoryManager.AddItemToInventory(myItemID, amountToTransfer);
                //CASH TRANSFER
                shopInventoryManager.storeCash += myModifiedItemValue * amountToTransfer;
                playerInventoryManager.playerCash -= myModifiedItemValue * amountToTransfer;
                //EVENT TRIGGER
                shopInventoryManager.OnInventoryCashChanged();
                playerInventoryManager.OnInventoryCashChanged();
            }else
            {
                shopScreenManager.OpenInsufficientPlayerFundsWarning();
                Debug.LogWarning($"You don't have enough cash to complete this transaction!");
            }

            transferingItem = false;
        }
    }
    public void CancelItemTransfer()
    {
        transferingItem = false;
    }
    //EVENT METHODS
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
        GetItemID();
        GetItemInformation();
        SetUIInformation();
    }
    public void OnInventoryItemRemoved(int removedItemID)
    {
        if (removedItemID == myItemID)
        {
            //Debug.Log($"Destroying {gameItemDictionary.gameItemNames[myItemID]} itemUI");
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
        GetItemID();
        GetItemInformation();
        SetUIInformation();
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
            shopScreenManager.OpenTransferAmountSelector(myItemAmount, myModifiedItemValue);
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
        //Debug.Log($"My order in list is {orderInList} and my item is {myItemName}");
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        ResetCell();
    }
}
