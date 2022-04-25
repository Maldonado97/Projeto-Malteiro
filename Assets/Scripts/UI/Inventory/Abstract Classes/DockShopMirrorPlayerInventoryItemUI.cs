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
    protected override void SubscribeToEvents()
    {
        playerInventoryManager.onInventoryChanged += OnInventoryChanged;
        playerInventoryManager.onInventoryItemAdded += OnInventoryItemAdded;
        playerInventoryManager.onInventoryItemRemoved += OnInventoryItemRemoved;
        playerInventoryManager.onSortModeChanged += OnSortModeChanged;
        shopScreenManager.onMirrorPlayerInventoryItemUIRemoved += OnInventoryItemUIRemoved;
        shopScreenManager.onItemTransferConfirmed += TransferMultipleItems;
        shopScreenManager.onItemTransferCanceled += CancelItemTransfer;
    }
    public override void GetItemInformation()
    {
        myItemID = playerInventoryManager.itemIDsInInventory[orderInList];
        myItemAmount = playerInventoryManager.itemAmount[myItemID];
        myItemName = gameItemDictionary.gameItemNames[myItemID];
        myItemValue = gameItemDictionary.gameItemValues[myItemID];
        myItemWeight = gameItemDictionary.gameItemWeights[myItemID];
    }
    public override void TransferSingleItem()
    {
        playerInventoryManager.RemoveItemFromInventory(myItemID, 1);
        shopInventoryManager.AddItemToInventory(myItemID, 1);
    }
    public override void TransferMultipleItems(int amountToTransfer)
    {
        if(transferingItem == true)
        {
            playerInventoryManager.RemoveItemFromInventory(myItemID, amountToTransfer);
            shopInventoryManager.AddItemToInventory(myItemID, amountToTransfer);
            transferingItem = false;
        }
    }
    protected override void OnDestroy()
    {
        shopScreenManager.OnMirrorPlayerInventoryItemUIRemoved(orderInList);
        UnsubscribeFromAllEvents();
    }
    protected override void UnsubscribeFromAllEvents()
    {
        playerInventoryManager.onInventoryChanged -= OnInventoryChanged;
        playerInventoryManager.onInventoryItemAdded -= OnInventoryItemAdded;
        playerInventoryManager.onInventoryItemRemoved -= OnInventoryItemRemoved;
        playerInventoryManager.onSortModeChanged -= OnSortModeChanged;
        shopScreenManager.onMirrorPlayerInventoryItemUIRemoved -= OnInventoryItemUIRemoved;
        shopScreenManager.onItemTransferConfirmed -= TransferMultipleItems;
        shopScreenManager.onItemTransferCanceled -= CancelItemTransfer;
    }
}
