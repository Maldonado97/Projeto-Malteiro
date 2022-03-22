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

    public event Action onItemSelected;
    private void Start()
    {
        instance = this;
    }
    public void CreateItemUI()
    {
        Instantiate(inventoryItemUI, inventoryItemUIParent.transform);
    }
    public void DisplayItemDescription(string itemDescription, float itemWeight, float itemValue)
    {
        itemDescriptionTM.text = itemDescription;
        itemWeightTM.text = itemWeight.ToString();
        itemValueTM.text = itemValue.ToString();
    }
}
