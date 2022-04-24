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

    protected Image cell;
    protected int orderInList;
    [HideInInspector] public int myItemID;
    [HideInInspector] public int myItemAmount;
    [HideInInspector] public string myItemName;

    protected void Start()
    {
        SetParentSingletonReferences();
        playerInventoryManager = PlayerInventoryManager.instance;
        gameItemDictionary = GameItemDictionary.instance;

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        SubscribeToEvents();
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
    public virtual void GetItemInformation()
    {
        myItemID = shopInventoryManager.itemIDsInInventory[orderInList];
        myItemAmount = shopInventoryManager.itemAmount[myItemID];
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
    public virtual void TransferItem(int amountToTransfer)
    {
        shopInventoryManager.RemoveItemFromInventory(myItemID, amountToTransfer);
        playerInventoryManager.AddItemToInventory(myItemID, amountToTransfer);
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
