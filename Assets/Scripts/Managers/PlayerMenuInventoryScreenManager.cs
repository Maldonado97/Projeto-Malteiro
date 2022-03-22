using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMenuInventoryScreenManager : MonoBehaviour
{
    public static PlayerMenuInventoryScreenManager instance;

    private GameItemDictionary gameItemDictionary;
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

        gameItemDictionary = GameItemDictionary.instance; //COULD BREAK CODE
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
        var GID = gameItemDictionary;
        int selectedItemID;

        SelectNewInventoryItem();
        inventoryItemUIScript.SelectThisItem();
        selectedItemID = inventoryItemUIScript.myitemID;
    }
    public void SelectNewInventoryItem()
    {
        onNewItemSelected?.Invoke();
    }
    public void DisplayItemDescription(string itemDescription, float itemWeight, float itemValue)
    {
        itemDescriptionTM.text = itemDescription;
        itemWeightTM.text = itemWeight.ToString();
        itemValueTM.text = itemValue.ToString();
    }
}
