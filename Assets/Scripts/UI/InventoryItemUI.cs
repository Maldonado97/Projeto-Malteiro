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
    private void Start()
    {
        cell = gameObject.GetComponent<Image>();
        orderInList = transform.GetSiblingIndex();
        Debug.Log(orderInList);
    }
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
