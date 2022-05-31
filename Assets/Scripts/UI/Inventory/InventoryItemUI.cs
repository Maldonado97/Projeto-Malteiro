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
    [HideInInspector] public int orderInList;
    [HideInInspector] public int myItemID;
    public string myItemName;
    public int myItemAmount;
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
        GetItemInformation();
        SetUIInformation();
        SubscribeToItemUIList();
    }
    protected virtual void SubscribeToItemUIList()
    {
        inventoryScreenManager.SubscribeItemUI(this);
    }
    public void SubscribeToEvents()
    {
        inventoryManager.onInventoryChanged += OnInventoryChanged;
        inventoryScreenManager.onAllItemsDeselected += DeselectThisItem;
    }
    public void GetItemID()
    {
        PlayerInventoryScreenManager.instance.GetItemUIItemID(this);
        //myItemID = inventoryManager.itemIDsInInventory[orderInList];
    }
    public virtual void GetItemInformation()
    {
        myItemAmount = PlayerInventoryManager.instance.itemAmount[myItemID];
        myItemName = GameItemDictionary.instance.gameItemNames[myItemID];
    }
    public void SetUIInformation()
    {
        itemNameTM.text = myItemName;
        itemAmountTM.text = $"{myItemAmount}";
    }
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
    public void UnsubscribeFromAllEvents()
    {
        inventoryManager.onInventoryChanged -= OnInventoryChanged;
        inventoryScreenManager.onAllItemsDeselected -= DeselectThisItem;
    }
    //SELECTION METHODS
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
    //DESTROY
    public void OnDestroy()
    {
        if (itemSelected)
        {
            inventoryScreenManager.DeselectAllItems();
        }
        //inventoryScreenManager.OnInventoryItemUIRemoved(orderInList);
        UnsubscribeFromAllEvents();
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
