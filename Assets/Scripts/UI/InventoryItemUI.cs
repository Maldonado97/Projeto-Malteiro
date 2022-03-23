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

        inventoryManager.onInventoryChanged += UpdateInformation;
        inventoryScreenManager.onAllItemsDeselected += DeselectThisItem;

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        GetItemID();
        SetUIInformation();
        //UpdateInformation(1);
    }
    public void UpdateInformation(int id) //Works even if parent is inactive!
    {
        CheckIfShouldRemoveSelf();
        if (!CheckIfShouldRemoveSelf())
        {
            //GetItemID();
            SetUIInformation();
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
    public bool CheckIfShouldRemoveSelf()
    {
        bool shouldRemoveSelf = false;

        //if((orderInList + 1) > inventoryManager.itemIDsInInventory.Count)
        {
            //shouldRemoveSelf = true;
            //RemoveSelf();
        }
        if (!inventoryManager.itemIDsInInventory.Contains(myitemID))
        {
            shouldRemoveSelf = true;
            RemoveSelf();
        }

        return shouldRemoveSelf;
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
    public void OnDestroy()
    {
        if (itemSelected)
        {
            inventoryScreenManager.DeselectAllItems();
        }
        inventoryManager.onInventoryChanged -= UpdateInformation;
        inventoryScreenManager.onAllItemsDeselected -= DeselectThisItem;
    }
    //USER INTERFACE METHODS
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        inventoryScreenManager.OnInventoryItemSelected(this);
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
    public void HighlightCell()
    {
        cell.color = highlightedCellColor;
    }
    public void ResetCell()
    {
        cell.color = defaultCellColor;
    }
    public void TestEvent()
    {
        itemNameTM.text = "Coke";
        itemAmountTM.text = "1000";
    }
}
