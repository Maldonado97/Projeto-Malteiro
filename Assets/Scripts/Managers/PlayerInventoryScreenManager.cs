using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventoryScreenManager : MonoBehaviour
{
    public static PlayerInventoryScreenManager instance;
    [Header("Item UI")]
    [SerializeField] GameObject inventoryItemUI;
    [Tooltip("Where the inventoryItemUI should spawn.")]
    [SerializeField] GameObject inventoryItemUIParent;
    [Header("Inventory Information Area")]
    [SerializeField] TextMeshProUGUI playerCashTM;
    [SerializeField] TextMeshProUGUI ownCashTM;
    [SerializeField] TextMeshProUGUI carryCapacityTM;
    [Header("Item Description Area")]
    [SerializeField] TextMeshProUGUI itemDescriptionTM;
    [SerializeField] TextMeshProUGUI itemWeightTM;
    [SerializeField] TextMeshProUGUI itemValueTM;

    [HideInInspector] public int selectedItemID;

    public event Action onAllItemsDeselected;
    public event Action<int> onInventoryItemUIRemoved;
    private void Start()
    {
        instance = this;
        selectedItemID = GameItemDictionary.instance.gameItemNames.Count;
        UpdateCarryCapacityText();
        UpdatePlayerCashText();

        PlayerInventoryManager.instance.onInventoryItemAdded += CreateItemUI;
        PlayerInventoryManager.instance.onInventoryWeightChanged += UpdateCarryCapacityText;
    }
    public void CreateItemUI()
    {
        Instantiate(inventoryItemUI, inventoryItemUIParent.transform);
    }
    public void DisplayItemDescription(string itemDescription, float itemWeight, float itemValue)
    {
        itemDescriptionTM.text = itemDescription;
        itemWeightTM.text = "Weight: " + itemWeight.ToString() + "Kg";
        itemValueTM.text = "Value: $" + itemValue.ToString();
    }
    public void DeselectAllItems()
    {
        onAllItemsDeselected?.Invoke();
        itemDescriptionTM.text = "-";
        itemWeightTM.text = "Weight: -";
        itemValueTM.text = "Value: -";
        selectedItemID = GameItemDictionary.instance.gameItemNames.Count + 1;
    }
    public void UpdateCarryCapacityText()
    {
        carryCapacityTM.text = $"Carry Capacity: {PlayerInventoryManager.instance.totalWeight}/{PlayerInventoryManager.instance.maxWeight}";
    }
    public void UpdatePlayerCashText()
    {
        playerCashTM.text = $"Cash: {PlayerInventoryManager.instance.playerCash}";
    }
    public void UpdateOwnCashText()
    {

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
