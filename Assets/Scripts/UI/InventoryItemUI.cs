using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : CustomButton
{
    [SerializeField] Color highlightedCellColor;
    [SerializeField] Color defaultCellColor;
    [SerializeField] TextMeshProUGUI itemNameTM;
    [SerializeField] TextMeshProUGUI itemAmountTM;
    private Image cell;
    private int orderInList;
    private int myitemID;
    private void Start()
    {
        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();
        PlayerInventoryManager.instance.onInventoryChanged += OnInventoryChanged;

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
        myitemID = PlayerInventoryManager.instance.itemIDsInInventory[orderInList];
    }
    public void SetUIInformation()
    {
        itemNameTM.text = GameItemDictionary.instance.gameItemNames[myitemID];
        itemAmountTM.text = PlayerInventoryManager.instance.itemAmount[myitemID].ToString();
    }
    public bool CheckIfShouldRemoveSelf()
    {
        bool shouldRemoveSelf = false;
        if((orderInList + 1) > PlayerInventoryManager.instance.itemIDsInInventory.Count)
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
    public void OnDestroy()
    {
        PlayerInventoryManager.instance.onInventoryChanged -= OnInventoryChanged;
    }
    //USER INTERFACE METHODS
    public void HighlightCell()
    {
        cell.color = highlightedCellColor;
    }
    public void ResetCell()
    {
        cell.color = defaultCellColor;
    }
    public void Test()
    {
        itemNameTM.text = "Coke";
        itemAmountTM.text = "1000";
    }
}
