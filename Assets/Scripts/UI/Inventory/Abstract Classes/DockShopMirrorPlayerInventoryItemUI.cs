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
    [HideInInspector] public PlayerInventoryManager parentInventoryManager;
    [HideInInspector] public GameItemDictionary gameItemDictionary;
    [Header("UI Elements")]
    [SerializeField] Color highlightedCellColor;
    [SerializeField] Color defaultCellColor;
    [SerializeField] TextMeshProUGUI itemNameTM;
    [SerializeField] TextMeshProUGUI itemAmountTM;

    private Image cell;
    protected int orderInList;
    [HideInInspector] public int myitemID;

    public void Start()
    {
        SetParentSingletonReferences();
        gameItemDictionary = GameItemDictionary.instance;

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        SubscribeToEvents();
        GetItemID();
        SetUIInformation();
    }
    protected abstract void SetParentSingletonReferences();
    protected void SubscribeToEvents()
    {
        parentInventoryManager.onInventoryChanged += OnInventoryChanged;
        parentInventoryManager.onInventoryItemAdded += OnInventoryItemAdded;
        parentInventoryManager.onInventoryItemRemoved += OnInventoryItemRemoved;
        parentInventoryManager.onSortModeChanged += OnSortModeChanged;
        shopScreenManager.onMirrorPlayerInventoryItemUIRemoved += OnInventoryItemUIRemoved;
    }
    public void OnInventoryChanged(int changedItemID)
    {
        if (changedItemID == myitemID)
        {
            SetUIInformation();
        }
    }
    public void OnInventoryItemAdded()
    {
        GetItemID();
        SetUIInformation();
    }
    public void OnInventoryItemRemoved(int removedItemID)
    {
        if (removedItemID == myitemID)
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
        GetItemID();
        SetUIInformation();
    }
    public void GetItemID()
    {
        myitemID = parentInventoryManager.itemIDsInInventory[orderInList];
    }
    public void SetUIInformation()
    {
        itemNameTM.text = gameItemDictionary.gameItemNames[myitemID];
        itemAmountTM.text = parentInventoryManager.itemAmount[myitemID].ToString();
    }
    public void HighlightCell()
    {
        cell.color = highlightedCellColor;
    }
    public void ResetCell()
    {
        cell.color = defaultCellColor;
    }
    protected void OnDestroy()
    {
        shopScreenManager.OnMirrorPlayerInventoryItemUIRemoved(orderInList);
        UnsubscribeFromAllEvents();
    }
    protected void UnsubscribeFromAllEvents()
    {
        parentInventoryManager.onInventoryChanged -= OnInventoryChanged;
        parentInventoryManager.onInventoryItemAdded -= OnInventoryItemAdded;
        parentInventoryManager.onInventoryItemRemoved -= OnInventoryItemRemoved;
        parentInventoryManager.onSortModeChanged -= OnSortModeChanged;
        shopScreenManager.onMirrorPlayerInventoryItemUIRemoved -= OnInventoryItemUIRemoved;
    }

    //USER INTERFACE METHODS
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
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
