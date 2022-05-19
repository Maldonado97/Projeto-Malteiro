using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class DockUIManager : MonoBehaviour
{
    public static DockUIManager instance;
    public GameObject dockMenu;
    [Header("Item Transfer Amount Selector")]
    private GeneralDockShopScreenManager transferingDockShop;
    public GameObject transferAmountSelector;
    public Slider selectorSlider;
    public TextMeshProUGUI selectedAmountTM;
    public TextMeshProUGUI transactionValueTM;
    [HideInInspector] public float transferingItemValue;

    public void Start()
    {
        SetInstance();
        dockMenu.SetActive(false);
        //foreach(GeneralDockShopScreenManager dockShopScreenManager in dockShopScreenManagers)
        //{
            //dockShopScreenManager.onIventoryItemUICreated += FlashStoreScreen;
        //}
    }
    protected abstract void SetInstance();
    public void OnPlayerDocked()
    {
        UIManager.instance.HUD.SetActive(false);
        dockMenu.SetActive(true);
    }
    public void FlashStoreScreen(GeneralDockShopScreenManager dockShopScreenManager)
    {
        //This method allows newly created ItemUIs to aquire their initial information. Without this,
        //newly created ItemUIs can't be removed unless the player opens the inventory screen where they are
        //located.
        if (!dockMenu.activeSelf)
        {
            dockMenu.SetActive(true);
            dockShopScreenManager.storeScreen.SetActive(true);
            dockShopScreenManager.storeScreen.SetActive(false);
            dockMenu.SetActive(false);
        }
        if(dockMenu.activeSelf && !dockShopScreenManager.storeScreen.activeSelf)
        {
            dockShopScreenManager.storeScreen.SetActive(true);
            dockShopScreenManager.storeScreen.SetActive(false);
        }
    }
    //TRANSFER AMOUNT SELECTOR
    public void OpenTransferAmountSelector(int itemAmount, float itemValue, GeneralDockShopScreenManager dockShop)
    {
        transferingDockShop = dockShop;
        selectorSlider.maxValue = itemAmount;
        transferingItemValue = itemValue;
        UpdateTransferAmountSelectorText();
        transferAmountSelector.SetActive(true);
    }
    public void CloseTransferAmountSelector()
    {
        transferAmountSelector.SetActive(false);
    }
    public void UpdateTransferAmountSelectorText()
    {
        selectedAmountTM.text = ("Choose Amount: " + selectorSlider.value);
        transactionValueTM.text = ($"Value: {Mathf.RoundToInt(selectorSlider.value * transferingItemValue)}");
    }
    public void ConfirmItemTransfer()
    {
        transferingDockShop.OnItemTransferConfirmed();
        selectorSlider.value = 1;
        CloseTransferAmountSelector();
    }
    public void CancelItemTransfer()
    {
        transferingDockShop.OnItemTransferCanceled();
        selectorSlider.value = 1;
        CloseTransferAmountSelector();
    }
}
