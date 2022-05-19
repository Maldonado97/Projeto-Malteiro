using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryItemUI : CustomButton
{
    [HideInInspector] public PlayerInventoryScreenManager inventoryScreenManager;
    [HideInInspector] public PlayerInventoryManager inventoryManager;
    [HideInInspector] public GameItemDictionary gameItemDictionary;
    [SerializeField] Color highlightedCellColor;
    [SerializeField] Color defaultCellColor;
    [SerializeField] TextMeshProUGUI itemNameTM;
    [SerializeField] TextMeshProUGUI itemAmountTM;
    private Image cell;
    private int orderInList;
    [HideInInspector] public int myitemID;
    [HideInInspector] public bool itemSelected;
    private void Awake()
    {
        inventoryScreenManager = PlayerInventoryScreenManager.instance;
        inventoryManager = PlayerInventoryManager.instance;
        gameItemDictionary = GameItemDictionary.instance;

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        SubscribeToEvents();
        GetItemID();
        SetUIInformation();
    }
    public void SubscribeToEvents()
    {
        inventoryManager.onInventoryChanged += OnInventoryChanged;
        inventoryManager.onInventoryItemAdded += OnInventoryItemAdded;
        inventoryManager.onInventoryItemRemoved += OnInventoryItemRemoved;
        inventoryManager.onSortModeChanged += OnSortModeChanged;
        inventoryScreenManager.onInventoryItemUIRemoved += OnInventoryItemUIRemoved;
        inventoryScreenManager.onAllItemsDeselected += DeselectThisItem;
    }
    public void OnInventoryChanged(int changedItemID)
    {
        if(changedItemID == myitemID)
        {
            SetUIInformation();
        }
    }
    public void OnInventoryItemAdded()
    {
        GetItemID();
        SetUIInformation();
        if (myitemID == inventoryScreenManager.selectedItemID)
        {
            inventoryScreenManager.OnInventoryItemSelected(this);
        }
    }
    public void OnInventoryItemRemoved(int removedItemID)
    {
        if(removedItemID == myitemID)
        {
            RemoveSelf();
        }
    }
    public void OnInventoryItemUIRemoved(int removedItemUIOrderInList) 
    {
        if(removedItemUIOrderInList < orderInList)
        {
            orderInList -= 1;
        }
    }
    public void OnSortModeChanged()
    {
        GetItemID();
        SetUIInformation();
        if (myitemID == inventoryScreenManager.selectedItemID)
        {
            inventoryScreenManager.OnInventoryItemSelected(this);
        }
    }
    public void GetItemID()
    {
        myitemID = inventoryManager.itemIDsInInventory[orderInList];
    }
    public void SetUIInformation()
    {
        itemNameTM.text = gameItemDictionary.gameItemNames[myitemID];
        itemAmountTM.text = inventoryManager.itemAmount[myitemID].ToString();
    }
    public void RemoveSelf()
    {
        Destroy(gameObject);
    }
    public void SelectThisItem()
    {
        itemSelected = true;
        HighlightCell();
    }
    public void DeselectThisItem()
    {
        itemSelected = false;
        ResetCell();
    }
    public void HighlightCell()
    {
        cell.color = highlightedCellColor;
    }
    public void ResetCell()
    {
        cell.color = defaultCellColor;
    }
    public void OnDestroy()
    {
        if (itemSelected)
        {
            inventoryScreenManager.DeselectAllItems();
        }
        inventoryScreenManager.OnInventoryItemUIRemoved(orderInList);
        UnsubscribeFromAllEvents();
    }
    public void UnsubscribeFromAllEvents()
    {
        inventoryManager.onInventoryChanged -= OnInventoryChanged;
        inventoryManager.onInventoryItemAdded -= OnInventoryItemAdded;
        inventoryManager.onInventoryItemRemoved -= OnInventoryItemRemoved;
        inventoryManager.onSortModeChanged -= OnSortModeChanged;
        inventoryScreenManager.onInventoryItemUIRemoved -= OnInventoryItemUIRemoved;
        inventoryScreenManager.onAllItemsDeselected -= DeselectThisItem;
    }
    //USER INTERFACE METHODS
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        inventoryScreenManager.OnInventoryItemSelected(this);
        //Debug.Log("My order in list is: " + orderInList + " and my item is " + gameItemDictionary.gameItemNames[myitemID]);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        HighlightCell();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (!itemSelected)
        {
            ResetCell();
        }
    }
}
