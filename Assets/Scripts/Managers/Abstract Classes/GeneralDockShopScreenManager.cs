using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class GeneralDockShopScreenManager : MonoBehaviour
{
    [Header("Item UIs")]
    [SerializeField] GameObject inventoryItemUI;
    [SerializeField] GameObject mirrorPlayerInventoryItemUI;
    [Header("Item UI Parents")]
    [Tooltip("Where own inventoryItemUI should spawn.")]
    [SerializeField] GameObject ownInventoryPanel;
    [Tooltip("Where the mirror player inventory item UI should spawn.")]
    [SerializeField] GameObject mirrorPlayerInventoryPanel;
    [Header("Item Transfer Amount Selector")]
    [SerializeField] GameObject transferAmountSelector;
    [SerializeField] GameObject selectorSliderTMPro;
    [SerializeField] GameObject selectorTMPro;

    private Slider selectorSlider;
    private TextMeshProUGUI selectorText;

    public event Action<int> onInventoryItemUIRemoved;
    public event Action<int> onMirrorPlayerInventoryItemUIRemoved;
    public event Action<int> onItemTransferConfirmed;
    public event Action onItemTransferCanceled;

    public void Start()
    {
        SetInstance();
        SubscribeToEvents();
        GetTransferAmountSelectorComponents();

        CloseTransferAmountSelector();
    }
    public abstract void SetInstance();
    public virtual void SubscribeToEvents()
    {
        PlayerInventoryManager.instance.onInventoryItemAdded += CreateMirrorPlayerInventoryItemUI;
    }
    public void GetTransferAmountSelectorComponents()
    {
        selectorSlider = selectorSliderTMPro.GetComponent<Slider>();
        selectorText = selectorTMPro.GetComponent<TextMeshProUGUI>();
    }
    public void CreateItemUI()
    {
        Instantiate(inventoryItemUI, ownInventoryPanel.transform);
    }
    public void CreateMirrorPlayerInventoryItemUI()
    {
        Instantiate(mirrorPlayerInventoryItemUI, mirrorPlayerInventoryPanel.transform);
    }
    public void OnInventoryItemUIRemoved(int removedItemUIOrderInList)
    {
        onInventoryItemUIRemoved?.Invoke(removedItemUIOrderInList);
    }
    public void OnMirrorPlayerInventoryItemUIRemoved(int removedItemUIOrderInList)
    {
        onMirrorPlayerInventoryItemUIRemoved?.Invoke(removedItemUIOrderInList);
    }
    //TRANSFER AMOUNT SELECTOR
    public void OpenTransferAmountSelector(int selectorSliderMaxValue)
    {
        selectorSlider.maxValue = selectorSliderMaxValue;
        transferAmountSelector.SetActive(true);
    }
    public void CloseTransferAmountSelector()
    {
        transferAmountSelector.SetActive(false);
    }
    public void UpdateTransferAmountSelectorText()
    {
        selectorText.text = ("Amount: " + selectorSlider.value);
    }
    public void ConfirmItemTransfer()
    {
        onItemTransferConfirmed?.Invoke(Mathf.RoundToInt(selectorSlider.value));
        selectorSlider.value = 1;
        CloseTransferAmountSelector();
    }
    public void CancelItemTransfer()
    {
        onItemTransferCanceled?.Invoke();
        selectorSlider.value = 1;
        CloseTransferAmountSelector();
    }
}
