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

    public int selectedItemID;

    public event Action onAllItemsDeselected;
    public event Action<int> onInventoryItemUIRemoved;
    private void Start()
    {
        instance = this;
        selectedItemID = GameItemDictionary.instance.gameItemNames.Count;
    }
    public void CreateItemUI(int itemID)
    {
        Instantiate(inventoryItemUI, inventoryItemUIParent.transform);
    }
    public void DisplayItemDescription(string itemDescription, float itemWeight, float itemValue)
    {
        itemDescriptionTM.text = itemDescription;
        itemWeightTM.text = "Weight: " + itemWeight.ToString();
        itemValueTM.text = "Value: " + itemValue.ToString();
    }
    public void DeselectAllItems()
    {
        onAllItemsDeselected?.Invoke();
        itemDescriptionTM.text = "-";
        itemWeightTM.text = "Weight: -";
        itemValueTM.text = "Value: -";
        selectedItemID = GameItemDictionary.instance.gameItemNames.Count + 1;
    }
    //EVENT METHODS
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

        DeselectAllItems();
        inventoryItemUIScript.SelectThisItem();
        selectedItemID = inventoryItemUIScript.myitemID;
        DisplayItemDescription(GID.gameItemDescriptions[selectedItemID], GID.gameItemWeights[selectedItemID],
            GID.gameItemValues[selectedItemID]);
    }
    public void OnInventoryItemUIRemoved(int removedItemUIOrderInList)
    {
        onInventoryItemUIRemoved?.Invoke(removedItemUIOrderInList);
    }
}
