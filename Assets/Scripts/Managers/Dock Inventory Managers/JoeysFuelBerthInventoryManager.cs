using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoeysFuelBerthInventoryManager : GeneralDockInventoryManager
{
    public static JoeysFuelBerthInventoryManager instance;

    public override void Start()
    {
        base.Start();

        //allowedItemTypes.Add("Fuel");
        desiredCash = 12000;

        customPlayerSubInventoryAllowedItems.Add(0); //Box of Steaks
        customPlayerSubInventoryAllowedItems.Add(4); //Fuel Drum
        CreateMirrorCustomSubPlayerInventory();
        JoeysFuelBerthScreenManager.instance.ChangePlayerInventoryFilter("Custom");
    }
    protected override void SetInstance()
    {
        instance = this;
    }

    protected override void SetStartingCash()
    {
        storeCash = 12000; ;
    }
    public override void AddInitialItems()
    {
        AddItemToInventory(4, Random.Range(1, 10));
    }
    public override void ShuffleInventory()
    {
        int desiredFuelAmount = 45;
        int fuelAmountUpperBound = Mathf.FloorToInt(desiredFuelAmount * (1 + desiredInventoryItemsVariance));
        int fuelAmountLowerBound = Mathf.CeilToInt(desiredFuelAmount * (1 - desiredInventoryItemsVariance));
        int maxFuelDelta = Mathf.CeilToInt(desiredFuelAmount * desiredInventoryItemsVariance);
        //FUEL DELTA SELECTION
        int fuelDelta;
        if (itemAmount[4] > fuelAmountUpperBound)
        {
            fuelDelta = UnityEngine.Random.Range(-maxFuelDelta, 0);
        }
        else if (itemAmount[4] < fuelAmountLowerBound)
        {
            fuelDelta = UnityEngine.Random.Range(0, maxFuelDelta);
        }
        else
        {
            fuelDelta = UnityEngine.Random.Range(-maxFuelDelta, maxFuelDelta);
        }
        //FUEL DELTA CORRECTION
        if (itemAmount[4] + fuelDelta < 0)
        {
            fuelDelta = -itemAmount[4] + 1;
        }
        //SHUFFLE
        if (canShuffleInventory)
        {
            if (fuelDelta > 0)
            {
                AddItemToInventory(4, fuelDelta);
            }
            else
            {
                fuelDelta *= -1;
                RemoveItemFromInventory(4, fuelDelta);
            }

            if (storeCash >= 20000)
            {
                storeCash += UnityEngine.Random.Range(-3000, 501);
                OnInventoryCashChanged();
            }
            if (storeCash >= 5000 && storeCash < 20000)
            {
                storeCash += UnityEngine.Random.Range(-2000, 2001);
                OnInventoryCashChanged();
            }
            if (storeCash < 5000 && storeCash >= 1000)
            {
                storeCash += UnityEngine.Random.Range(-500, 3001);
                OnInventoryCashChanged();
            }
            if (storeCash < 1000)
            {
                storeCash += UnityEngine.Random.Range(2000, 5001);
                OnInventoryCashChanged();
            }
        }
        StartCoroutine(StartInventoryShuffleCounter(UnityEngine.Random.Range(180, 301))); //3(180) to 5(300) minutes.
        //StartCoroutine(StartInventoryShuffleCounter(1));
    }
}
