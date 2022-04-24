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
    }
    public override void GetItemInformation()
    {
        myItemID = playerInventoryManager.itemIDsInInventory[orderInList];
        myItemAmount = playerInventoryManager.itemAmount[myItemID];
        myItemName = gameItemDictionary.gameItemNames[myItemID];
    }
    public override void TransferItem(int amountToTransfer)
    {
        playerInventoryManager.RemoveItemFromInventory(myItemID, amountToTransfer);
        shopInventoryManager.AddItemToInventory(myItemID, amountToTransfer);
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
    }
}
