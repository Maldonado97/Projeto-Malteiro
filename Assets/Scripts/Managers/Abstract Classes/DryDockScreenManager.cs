using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public abstract class DryDockScreenManager : MonoBehaviour
{
    [SerializeField] GameObject dryDockScreen;
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI hullIntegrityText;
    [SerializeField] TextMeshProUGUI playerCashText;
    [SerializeField] TextMeshProUGUI serviceNameText;
    [SerializeField] TextMeshProUGUI serviceDescriptionText;
    [SerializeField] TextMeshProUGUI serviceValueText;
    [SerializeField] BoxButton confirmButton;
    [Tooltip("Where the service option should pop up")]
    [SerializeField] GameObject serviceListGameObject;
    [SerializeField] GameObject serviceOptionPrefab;
    [Header("Warnings")]
    [SerializeField] GameObject warningBox;
    [SerializeField] TextMeshProUGUI warningText; 

    List<DryDockServiceOption> serviceOptions = new List<DryDockServiceOption>();
    string selectedServiceName = "None";
    DryDockServiceOption selectedServiceOption;
    float perPointRepairPrice = 1.2f;
    float totalRepairPriceDiscount = .9f;
    float partialRepairAmount;
    float totalRepairAmount;
    float partialRepairPrice;
    float totalRepairPrice;
    private bool dryDockScreenOpen = false;
    private bool updatedServicesOnScreenOpen = false;
    //private string selectedServiceDescription;
    //private float seelctedServiceValue;
    //SERVICE LISTS
    [HideInInspector] public List<string> serviceNames = new List<string>();
    [HideInInspector] public List<string> serviceDescriptions = new List<string>();
    [HideInInspector] public List<float> serviceValues = new List<float>();

    public void Awake()
    {
        SetInstance();
    }
    public void Start()
    {
        AddServices();
        foreach(string serviceName in serviceNames)
        {
            Instantiate(serviceOptionPrefab, serviceListGameObject.transform);
        }
        PlayerControl.instance.onDamageTaken += UpdateServices;
        confirmButton.onButtonClicked += ExecuteService;
    }
    public void Update()
    {
        if(dryDockScreen.activeInHierarchy == true)
        {
            //Debug.Log("Dry Dock Screen Open");
            dryDockScreenOpen = true;
            if(updatedServicesOnScreenOpen == false)
            {
                Debug.Log("Updating on dry dock screen open");
                hullIntegrityText.text = $"Your Hull Integrity: {PlayerControl.instance.health}/{PlayerControl.instance.maxHealth}";
                playerCashText.text = $"Your Money: {PlayerInventoryManager.instance.playerCash}";
                UpdateServices();
                updatedServicesOnScreenOpen = true;
            }
        }
        else
        {
            //Debug.Log("Dry Dock Screen Closed");
            DeselectAllServiceOptions();
            serviceNameText.text = "-";
            serviceDescriptionText.text = "-";
            serviceValueText.text = "Value: -";
            dryDockScreenOpen = false;
            updatedServicesOnScreenOpen = false;
        }
        
    }
    public abstract void SetInstance();
    public void AddServices()
    {
        //ID: 0, Total Repair
        serviceNames.Add("Total Repair");
        serviceDescriptions.Add("Totally Repair Hull.");
        serviceValues.Add(10);
        //ID: 1, Partial Repair
        serviceNames.Add("Partial Repair");
        serviceDescriptions.Add("Repair 25% of hull integrity.");
        serviceValues.Add(10);
        //ID: 2, Reinforce Hull
        //serviceNames.Add("Reinforce Hull");
        //serviceDescriptions.Add("Temporarily raise maximum hull integrity.");
        //serviceValues.Add(500);

        UpdateServices();
    }
    public void IntializeServiceOption(DryDockServiceOption serviceOption)
    {
        int orderInList = serviceOption.orderInList;

        serviceOptions.Add(serviceOption);
        serviceOption.name = serviceNames[orderInList];
        serviceOption.serviceName = serviceNames[orderInList];
        //serviceOption.buttonTextTMPro.text = serviceNames[orderInList];
        serviceOption.serviceDescription = serviceDescriptions[orderInList];
        serviceOption.serviceValue = serviceValues[orderInList];
        //serviceOption.serviceValueText.text = $"{serviceValues[orderInList]}";
        serviceOption.onServiceClicked += OnServiceOptionClicked;
    }
    public void OnServiceOptionClicked(DryDockServiceOption serviceOption)
    {
        DeselectAllServiceOptions();
        SelectServiceOption(serviceOption);
        DisplayServiceDescription(serviceOption);
    }
    public void DeselectAllServiceOptions()
    {
        foreach (DryDockServiceOption serviceOption in serviceOptions)
        {
            serviceOption.selected = false;
            serviceOption.UnhighlightButton();
        }
        selectedServiceName = "None";
    }
    public void SelectServiceOption(DryDockServiceOption serviceOption)
    {
        serviceOption.selected = true;
        serviceOption.SelectButton();
        selectedServiceOption = serviceOption;
        selectedServiceName = serviceOption.serviceName;
    }
    public void DisplayServiceDescription(DryDockServiceOption serviceOption)
    {
        serviceDescriptionText.text = serviceOption.serviceDescription;
        serviceValueText.text = $"Value: {serviceOption.serviceValue}";
        serviceNameText.text = $"{serviceOption.name}";
    }
    public void CalculateAmountToRepair()
    {
        var playerControl = PlayerControl.instance;
        //AMOUNT TO REPAIR
        partialRepairAmount = playerControl.maxHealth * .25f; //In Hull Points
        if (playerControl.health + partialRepairAmount > playerControl.maxHealth)
        {
            partialRepairAmount = playerControl.maxHealth - playerControl.health;
            totalRepairPriceDiscount = 1;
        }
        totalRepairAmount = playerControl.maxHealth - playerControl.health;
        //Debug.Log($"New total repair amount = {totalRepairAmount}");
        //Debug.Log($"New partial repair amount = {partialRepairAmount}");
    }
    public void UpdateServicePrices()
    {
        //REPAIR PRICE
        totalRepairPrice = totalRepairAmount * perPointRepairPrice * totalRepairPriceDiscount;
        partialRepairPrice = partialRepairAmount * perPointRepairPrice;
        
        serviceValues[0] = Mathf.RoundToInt(totalRepairPrice);
        serviceDescriptions[0] = $"Totally Repair Hull ({totalRepairAmount} hull points).";
        serviceValues[1] = Mathf.RoundToInt(partialRepairPrice);
        serviceDescriptions[1] = $"Repair 25% of hull integrity. ({partialRepairAmount} hull points).";

        foreach (DryDockServiceOption serviceOption in serviceOptions)
        {
            if(serviceOption.serviceName == "Total Repair" || serviceOption.serviceName == "Partial Repair")
            {
                serviceOption.serviceValue = serviceValues[serviceOption.orderInList];
                serviceOption.serviceDescription = serviceDescriptions[serviceOption.orderInList];
                serviceOption.displayServiceOptionInformation();
            }
        }
        if(selectedServiceName != "None")
        {
            DisplayServiceDescription(selectedServiceOption);
        }
    }
    public void UpdateServices()
    {
        CalculateAmountToRepair();
        UpdateServicePrices();
        //Debug.Log("Repair services updated");
    }
    public void ExecuteService()
    {
        var playerControl = PlayerControl.instance;
        var playerInventory = PlayerInventoryManager.instance;

        if(totalRepairAmount > 0)
        {
            if (selectedServiceName == "Total Repair")
            {
                if(playerInventory.playerCash >= totalRepairPrice)
                {
                    playerControl.health += totalRepairAmount;
                    playerInventory.RemoveCashFromInventory(totalRepairPrice);
                    Debug.Log("Totally repaired player hull");
                }
                else
                {
                    warningBox.SetActive(true);
                    warningText.text = $"You don't have enough money to complete this repair.";
                    //Debug.Log("Player doesn't have enough money to repair ship");
                }
            }
            if (selectedServiceName == "Partial Repair")
            {
                if (playerInventory.playerCash >= totalRepairPrice)
                {
                    playerControl.health += partialRepairAmount;
                    playerInventory.RemoveCashFromInventory(partialRepairPrice);
                    Debug.Log("Partialy repaired player hull");
                }
                else
                {
                    warningBox.SetActive(true);
                    warningText.text = $"You don't have enough money to complete this repair.";
                    //Debug.Log("Player doesn't have enough money to repair ship");
                }
                    
            }
            hullIntegrityText.text = $"Your Hull Integrity: {PlayerControl.instance.health}/{PlayerControl.instance.maxHealth}";
            playerCashText.text = $"Your Money: {PlayerInventoryManager.instance.playerCash}";
            UIManager.instance.UpdateHealthBar();
            UpdateServices();
        }
        else
        {
            warningBox.SetActive(true);
            warningText.text = $"Hull already fully repaired.";
        }
        
    }
}
