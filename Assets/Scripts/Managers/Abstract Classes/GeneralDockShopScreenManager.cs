using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralDockShopScreenManager : MonoBehaviour
{
    [Header("Item UIs")]
    [SerializeField] GameObject inventoryItemUI;
    [SerializeField] GameObject mirrorPlayerInventoryItemUI;
    [Header("Item UI Parents")]
    [Tooltip("Where own inventoryItemUI should spawn.")]
    [SerializeField] GameObject ownInventoryPanel;
    [Tooltip("Where the mirror player inventory item UI should spawn.")]
    [SerializeField] GameObject mirrorPlayerInventoryPanel;

    public event Action<int> onInventoryItemUIRemoved;
    public event Action<int> onMirrorPlayerInventoryItemUIRemoved;

    public void Start()
    {
        SetInstance();
        SubscribeToEvents();
    }
    public abstract void SetInstance();
    public virtual void SubscribeToEvents()
    {
        PlayerInventoryManager.instance.onInventoryItemAdded += CreateMirrorPlayerInventoryItemUI;
    }
    public void CreateItemUI()
    {
        Instantiate(inventoryItemUI, ownInventoryPanel.transform);
    }
    public void CreateMirrorPlayerInventoryItemUI()
    {
        Instantiate(mirrorPlayerInventoryItemUI, mirrorPlayerInventoryPanel.transform);
    }
    public void OnInventoryItemUIRemoved(int removedItemUIOrderInList)
    {
        onInventoryItemUIRemoved?.Invoke(removedItemUIOrderInList);
    }
    public void OnMirrorPlayerInventoryItemUIRemoved(int removedItemUIOrderInList)
    {
        onMirrorPlayerInventoryItemUIRemoved?.Invoke(removedItemUIOrderInList);
    }
}
