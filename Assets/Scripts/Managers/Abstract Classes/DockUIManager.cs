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
    [Header("Insufficient Cash Warning")]
    [SerializeField] GameObject insufficientFundsWarning;
    [SerializeField] TextMeshProUGUI insufficientFundsWarningText;

    public void Start()
    {
        SetInstance();
        dockMenu.SetActive(false);
    }
    protected abstract void SetInstance();
    public void OnPlayerDocked() //Player Docking and undocking handled by player control script.
    {
        dockMenu.SetActive(true);
    }
    public void FlashStoreScreen(GeneralDockShopScreenManager dockShopScreenManager)
    {
        //This method allows newly created ItemUIs to aquire their initial information. Without this,
        //newly created ItemUIs can't be removed unless the player opens the inventory screen where they are
        //located.
        if (!dockMenu.activeSelf)
        {
            //Debug.Log("Flashing store screen");
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
    //INSUFFICIENT FUNDS WARNING
    public void OpenInsufficientPlayerFundsWarning()
    {
        insufficientFundsWarningText.text= $"You don't have enough funds to complete this transaction!";
        insufficientFundsWarning.SetActive(true);
    }
    public void OpenInsufficientStoreFundsWarning()
    {
        insufficientFundsWarningText.text = $"Store doesn't have enough funds to complete this transaction!";
        insufficientFundsWarning.SetActive(true);
    }
}
