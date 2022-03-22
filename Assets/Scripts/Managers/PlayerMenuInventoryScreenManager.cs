using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMenuInventoryScreenManager : MonoBehaviour
{
    public static PlayerMenuInventoryScreenManager instance;

    public GameObject inventoryItemUI;
    [Tooltip("Where the inventoryItemUI should spawn.")]
    public GameObject inventoryItemUIParent;
    [Header("Description Area")]
    public TextMeshProUGUI itemDescriptionTM;
    public TextMeshProUGUI itemWeightTM;
    public TextMeshProUGUI itemValueTM;

    [HideInInspector] public List<InventoryItemUI> inventoryItems;

    public event Action onNewItemSelected;
    private void Start()
    {
        instance = this;
    }
    public void CreateItemUI()
    {
        Instantiate(inventoryItemUI, inventoryItemUIParent.transform);
    }
    public void SubscribeItemUI(InventoryItemUI inventoryItem)
    {
        if(inventoryItems == null)
        {
            inventoryItems = new List<InventoryItemUI>();
        }
        inventoryItems.Add(inventoryItem);
    }
    public void UnsubscribeItemUI(InventoryItemUI inventoryItem)
    {
        inventoryItems.Remove(inventoryItem);
    }
    public void OnInventoryItemEnter(InventoryItemUI inventoryItem)
    {

    }
    public void OnInventoryItemExit(InventoryItemUI inventoryItem)
    {

    }
    public void OnInventoryItemSelected(InventoryItemUI inventoryItem)
    {
        var inventoryItemUIScript = inventoryItem.GetComponent<InventoryItemUI>();
        var GID = GameItemDictionary.instance;
        int selectedItemID;

        SelectNewInventoryItem();
        inventoryItemUIScript.SelectThisItem();
        selectedItemID = inventoryItemUIScript.myitemID;
        DisplayItemDescription(GID.gameItemDescriptions[selectedItemID], GID.gameItemWeights[selectedItemID],
            GID.gameItemValues[selectedItemID]);
    }
    public void SelectNewInventoryItem()
    {
        onNewItemSelected?.Invoke();
    }
    public void DisplayItemDescription(string itemDescription, float itemWeight, float itemValue)
    {
        itemDescriptionTM.text = itemDescription;
        itemWeightTM.text = "Weight: " + itemWeight.ToString();
        itemValueTM.text = "Value: " + itemValue.ToString();
    }
}
