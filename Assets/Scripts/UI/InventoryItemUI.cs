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
        inventoryScreenManager = PlayerMenuInventoryScreenManager.instance; //COULD BREAK CODE
        inventoryManager = PlayerInventoryManager.instance; //COULD BREAK CODE
        gameItemDictionary = GameItemDictionary.instance;

        inventoryManager.onInventoryChanged += OnInventoryChanged;
        inventoryScreenManager.onNewItemSelected += DeselectThisItem;
        inventoryScreenManager.SubscribeItemUI(this);

        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();

        UpdateInformation();
    }
    public void OnInventoryChanged(int id)
    {
        UpdateInformation();
        //Debug.Log("Info updated");
    }
    public void UpdateInformation()
    {
        CheckIfShouldRemoveSelf();
        if (!CheckIfShouldRemoveSelf())
        {
            GetItemID();
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
        if((orderInList + 1) > inventoryManager.itemIDsInInventory.Count)
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
        inventoryManager.onInventoryChanged -= OnInventoryChanged;
        inventoryScreenManager.onNewItemSelected -= DeselectThisItem;
        inventoryScreenManager.UnsubscribeItemUI(this);
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
