using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public abstract class DockShopMirrorPlayerInventoryItemUI : DockShopInventoryItemUI
{
    protected override abstract void SetParentSingletonReferences();
    protected override void SubscribeToItemUIList()
    {
        shopScreenManager.SubscribeMirrorItemUI(this);
    }
    protected override void SubscribeToEvents()
    {
        shopScreenManager.onItemTransferConfirmed += TransferMultipleItems;
        shopScreenManager.onItemTransferCanceled += CancelItemTransfer;
    }
    public override void GetItemID()
    {
        shopScreenManager.GetMirrorItemUIItemID(this);
    }
    public override void GetItemInformation()
    {
        myItemAmount = PlayerInventoryManager.instance.itemAmount[myItemID];
        myItemName = GameItemDictionary.instance.gameItemNames[myItemID];
        myItemValue = GameItemDictionary.instance.gameItemValues[myItemID];
        myItemWeight = GameItemDictionary.instance.gameItemWeights[myItemID];
        //MODIFIED ITEM VALUE
        myModifiedItemValue = myItemValue + myItemValue * (baseValueModification + itemValueModifications[myItemName]);
    }
    public override void TransferSingleItem()
    {
        var playerInventoryManager = PlayerInventoryManager.instance;
        if (shopInventoryManager.storeCash >= myModifiedItemValue)
        {
            //ITEM TRANSFER
            Debug.Log($"Removing {GameItemDictionary.instance.gameItemNames[myItemID]} from inventory");
            playerInventoryManager.RemoveItemFromInventory(myItemID, 1);
            shopInventoryManager.AddItemToInventory(myItemID, 1);
            //CASH TRANSFER
            playerInventoryManager.playerCash += myModifiedItemValue;
            shopInventoryManager.storeCash -= myModifiedItemValue;
            //EVEN TRIGGER
            playerInventoryManager.OnInventoryCashChanged();
            shopInventoryManager.OnInventoryCashChanged();
        }
        else
        {
            shopScreenManager.OpenInsufficientStoreFundsWarning();
            Debug.LogWarning($"Store doesn't have enough cash to complete this transaction!");
        }
    }
    public override void TransferMultipleItems(int amountToTransfer)
    {
        var playerInventoryManager = PlayerInventoryManager.instance;
        if (transferingItem == true)
        {
            if(shopInventoryManager.storeCash >= myModifiedItemValue * amountToTransfer)
            {
                //ITEM TRANSFER
                playerInventoryManager.RemoveItemFromInventory(myItemID, amountToTransfer);
                shopInventoryManager.AddItemToInventory(myItemID, amountToTransfer);
                //CASH TRANSFER
                playerInventoryManager.playerCash += myModifiedItemValue * amountToTransfer;
                shopInventoryManager.storeCash -= myModifiedItemValue * amountToTransfer;
                //EVENT TRIGGER
                playerInventoryManager.OnInventoryCashChanged();
                shopInventoryManager.OnInventoryCashChanged();
            }else
            {
                shopScreenManager.OpenInsufficientStoreFundsWarning();
                Debug.LogWarning($"Store doesn't have enough cash to complete this transaction!");
            }

            transferingItem = false;
        }
    }
    protected override void OnDestroy()
    {
        UnsubscribeFromAllEvents();
    }
    protected override void UnsubscribeFromAllEvents()
    {
        shopScreenManager.onItemTransferConfirmed -= TransferMultipleItems;
        shopScreenManager.onItemTransferCanceled -= CancelItemTransfer;
    }
}
