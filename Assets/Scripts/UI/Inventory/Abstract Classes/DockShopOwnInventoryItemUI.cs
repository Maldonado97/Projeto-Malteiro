using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public abstract class DockShopOwnInventoryItemUI : CustomButton
{
    [HideInInspector] public GeneralDockShopScreenManager shopScreenManager;
    [HideInInspector] public GeneralDockInventoryManager parentInventoryManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public GameItemDictionary gameItemDictionary;
    [Header("UI Elements")]
    [SerializeField] Color highlightedCellColor;
    [SerializeField] Color defaultCellColor;
    [SerializeField] TextMeshProUGUI itemNameTM;
    [SerializeField] TextMeshProUGUI itemAmountTM;

    private Image cell;
    protected int orderInList;
    [HideInInspector] public int myItemID;
    [HideInInspector] public int myItemAmount;
    [HideInInspector] public string myItemName;
    public void Start()
    {
        SetParentSingletonReferences();
        gameItemDictionary = GameItemDictionary.instance;
        playerInventoryManager = PlayerInventoryManager.instance;

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        SubscribeToEvents();
        GetItemInformation();
        SetUIInformation();
    }
    protected abstract void SetParentSingletonReferences();
    protected virtual void SubscribeToEvents() //PROBLEMATIC CODE!!!!!!!!
    {
        parentInventoryManager.onInventoryChanged += OnInventoryChanged;
        parentInventoryManager.onInventoryItemAdded += OnInventoryItemAdded;
        parentInventoryManager.onInventoryItemRemoved += OnInventoryItemRemoved;
        parentInventoryManager.onSortModeChanged += OnSortModeChanged;
        shopScreenManager.onInventoryItemUIRemoved += OnInventoryItemUIRemoved;
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
    public void GetItemInformation()
    {
        myItemID = parentInventoryManager.itemIDsInInventory[orderInList];
        myItemAmount = parentInventoryManager.itemAmount[myItemID];
        myItemName = gameItemDictionary.gameItemNames[myItemID];
    }
    public void SetUIInformation()
    {
        itemNameTM.text = myItemName;
        itemAmountTM.text = myItemAmount.ToString();
    }
    public void HighlightCell()
    {
        cell.color = highlightedCellColor;
    }
    public void ResetCell()
    {
        cell.color = defaultCellColor;
    }
    public void TransferItem(int amountToTransfer)
    {
        parentInventoryManager.RemoveItemFromInventory(myItemID, amountToTransfer);
        playerInventoryManager.AddItemToInventory(myItemID, amountToTransfer);
    }
    public virtual void OnDestroy()
    {
        shopScreenManager.OnInventoryItemUIRemoved(orderInList);
        UnsubscribeFromAllEvents();
    }
    protected virtual void UnsubscribeFromAllEvents()
    {
        parentInventoryManager.onInventoryChanged -= OnInventoryChanged;
        parentInventoryManager.onInventoryItemAdded -= OnInventoryItemAdded;
        parentInventoryManager.onInventoryItemRemoved -= OnInventoryItemRemoved;
        parentInventoryManager.onSortModeChanged -= OnSortModeChanged;
        shopScreenManager.onInventoryItemUIRemoved -= OnInventoryItemUIRemoved;
    }

    //USER INTERFACE METHODS
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        TransferItem(1);
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
