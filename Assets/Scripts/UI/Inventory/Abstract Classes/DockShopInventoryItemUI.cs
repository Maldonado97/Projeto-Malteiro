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
    [Header("UI Elements")]
    [SerializeField] protected Color highlightedCellColor = new Color(87, 101, 116, 200);
    [SerializeField] protected Color defaultCellColor = new Color(87, 101, 116, 0);
    [SerializeField] protected TextMeshProUGUI itemNameTM;
    [SerializeField] protected TextMeshProUGUI itemAmountTM;
    [SerializeField] protected TextMeshProUGUI itemValueTM;
    [SerializeField] protected TextMeshProUGUI itemWeightTM;

    protected Image cell;
    public int orderInList;
    public int myItemID;
    public string myItemName;
    public int myItemAmount;
    [HideInInspector] public float myItemValue;
    public float myModifiedItemValue;
    public float myItemWeight;

    //protected List<float> itemValueModifications = new List<float>();
    protected Dictionary<string, float> itemValueModifications = new Dictionary<string, float>();
    protected float baseValueModification;
    
    protected bool transferingItem = false;

    protected virtual void Awake()
    {
        SetParentSingletonReferences();

        
        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        SubscribeToEvents();
        SetItemValueModifications();
        GetItemID();
        GetItemInformation();
        SetUIInformation();
        SubscribeToItemUIList();
    }
    protected abstract void SetParentSingletonReferences();
    protected virtual void SubscribeToItemUIList()
    {
        shopScreenManager.SubscribeItemUI(this);
    }
    protected virtual void SubscribeToEvents()
    {
        shopScreenManager.onItemTransferConfirmed += TransferMultipleItems;
        shopScreenManager.onItemTransferCanceled += CancelItemTransfer;
    }
    protected abstract void SetItemValueModifications();
    public virtual void GetItemID() //Leave this separate please...
    {
        shopScreenManager.GetItemUIItemID(this);
        //Debug.Log($"My order in list is {orderInList}, and my itemID is {myItemID}");
    }
    public virtual void GetItemInformation()
    {
        myItemAmount = shopInventoryManager.itemAmount[myItemID];
        myItemName = GameItemDictionary.instance.gameItemNames[myItemID];
        myItemValue = GameItemDictionary.instance.gameItemValues[myItemID];
        myItemWeight = GameItemDictionary.instance.gameItemWeights[myItemID];
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
        var playerInventoryManager = PlayerInventoryManager.instance;
        if (playerInventoryManager.playerCash >= myModifiedItemValue)
        {
            //ITEM TRANSFER
            shopInventoryManager.RemoveItemFromInventory(myItemID, 1);
            playerInventoryManager.AddItemToInventory(myItemID, 1);
            //CASH TRANSFER
            shopInventoryManager.AddCashToInventory(myModifiedItemValue);
            //shopInventoryManager.storeCash += myModifiedItemValue;
            playerInventoryManager.RemoveCashFromInventory(myModifiedItemValue);
            //playerInventoryManager.playerCash -= myModifiedItemValue;
            //EVEN TRIGGER
            //playerInventoryManager.OnInventoryCashChanged();
            //shopInventoryManager.OnInventoryCashChanged();
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
        var playerInventoryManager = PlayerInventoryManager.instance;
        if (transferingItem == true)
        {
            if(playerInventoryManager.playerCash >= myModifiedItemValue * amountToTransfer)
            {
                //ITEM TRANSFER
                shopInventoryManager.RemoveItemFromInventory(myItemID, amountToTransfer);
                playerInventoryManager.AddItemToInventory(myItemID, amountToTransfer);
                //CASH TRANSFER
                //shopInventoryManager.storeCash += myModifiedItemValue * amountToTransfer;
                //playerInventoryManager.playerCash -= myModifiedItemValue * amountToTransfer;
                //EVENT TRIGGER
                //shopInventoryManager.OnInventoryCashChanged();
                //playerInventoryManager.OnInventoryCashChanged();

                shopInventoryManager.AddCashToInventory(myModifiedItemValue * amountToTransfer);
                playerInventoryManager.RemoveCashFromInventory(myModifiedItemValue * amountToTransfer);
            }
            else
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
            GetItemInformation(); //change this to RefreshItemUI() just for organization's sake.
            SetUIInformation();
        }
    }
    public void RefreshItemUI()
    {
        GetItemID();
        GetItemInformation();
        SetUIInformation();
    }
    protected virtual void OnDestroy()
    {
        UnsubscribeFromAllEvents();
    }
    protected virtual void UnsubscribeFromAllEvents()
    {
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
