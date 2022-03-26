using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryItemUI : CustomButton
{
    [HideInInspector] public PlayerMenuInventoryScreenManager inventoryScreenManager;
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
    private void Start()
    {
        inventoryScreenManager = PlayerMenuInventoryScreenManager.instance;
        inventoryManager = PlayerInventoryManager.instance;
        gameItemDictionary = GameItemDictionary.instance;

        //inventoryManager.onInventoryChanged += UpdateInformation;
        inventoryManager.onInventoryChanged += OnInventoryChanged;
        inventoryManager.onInventoryItemAdded += OnInventoryItemAdded;
        inventoryManager.onInventoryItemRemoved += OnInventoryItemRemoved;
        inventoryManager.onSortModeChanged += OnSortModeChanged;
        inventoryScreenManager.onInventoryItemUIRemoved += OnInventoryItemUIRemoved;
        inventoryScreenManager.onAllItemsDeselected += DeselectThisItem;

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        GetItemID();
        SetUIInformation();
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
        //orderInList = transform.GetSiblingIndex();
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
    public void OnInventoryItemUIRemoved(int removedItemUIOrderInList) //USELESS. UPDATES ORDER IN LIST WHILE REMOVED ITEM UI IS STILL THERE
    {
        if(removedItemUIOrderInList < orderInList)
        {
            orderInList -= 1;
        }
        //orderInList = transform.GetSiblingIndex();
        //SetUIInformation();
    }
    public void OnSortModeChanged()
    {
        //orderInList = transform.GetSiblingIndex();
        GetItemID();
        SetUIInformation();
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
        inventoryManager.onInventoryChanged -= OnInventoryChanged;
        inventoryManager.onInventoryItemAdded -= OnInventoryItemAdded;
        inventoryManager.onInventoryItemRemoved -= OnInventoryItemRemoved;
        inventoryManager.onSortModeChanged -= OnSortModeChanged;
        inventoryScreenManager.onInventoryItemUIRemoved -= OnInventoryItemUIRemoved;
        inventoryScreenManager.onAllItemsDeselected -= DeselectThisItem;
        inventoryScreenManager.OnInventoryItemUIRemoved(orderInList); //This doesn't work because item is still active when it's invoked
    }
    //USER INTERFACE METHODS
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        inventoryScreenManager.OnInventoryItemSelected(this);
        Debug.Log("My order in list is: " + orderInList + " and my item is " + gameItemDictionary.gameItemNames[myitemID]);
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
