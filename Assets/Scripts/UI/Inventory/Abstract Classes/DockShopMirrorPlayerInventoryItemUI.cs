using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public abstract class DockShopMirrorPlayerInventoryItemUI : CustomButton
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
        playerInventoryManager = PlayerInventoryManager.instance;
        gameItemDictionary = GameItemDictionary.instance;

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        SubscribeToEvents();
        GetItemInformation();
        SetUIInformation();
    }
    protected abstract void SetParentSingletonReferences();
    protected void SubscribeToEvents()
    {
        playerInventoryManager.onInventoryChanged += OnInventoryChanged;
        playerInventoryManager.onInventoryItemAdded += OnInventoryItemAdded;
        playerInventoryManager.onInventoryItemRemoved += OnInventoryItemRemoved;
        playerInventoryManager.onSortModeChanged += OnSortModeChanged;
        shopScreenManager.onMirrorPlayerInventoryItemUIRemoved += OnInventoryItemUIRemoved;
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
        myItemID = playerInventoryManager.itemIDsInInventory[orderInList];
        myItemAmount = playerInventoryManager.itemAmount[myItemID];
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
        playerInventoryManager.RemoveItemFromInventory(myItemID, amountToTransfer);
        parentInventoryManager.AddItemToInventory(myItemID, amountToTransfer);
    }
    protected void OnDestroy()
    {
        shopScreenManager.OnMirrorPlayerInventoryItemUIRemoved(orderInList);
        UnsubscribeFromAllEvents();
    }
    protected void UnsubscribeFromAllEvents()
    {
        playerInventoryManager.onInventoryChanged -= OnInventoryChanged;
        playerInventoryManager.onInventoryItemAdded -= OnInventoryItemAdded;
        playerInventoryManager.onInventoryItemRemoved -= OnInventoryItemRemoved;
        playerInventoryManager.onSortModeChanged -= OnSortModeChanged;
        shopScreenManager.onMirrorPlayerInventoryItemUIRemoved -= OnInventoryItemUIRemoved;
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
