using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopeCityShop1InventoryManager : GeneralDockInventoryManager
{
    public static HopeCityShop1InventoryManager instance;

    public override void Start()
    {
        base.Start();

        desiredInventoryItems = 5;
    }
    protected override void SetInstance()
    {
        instance = this;
    }

    protected override void SetStartingCash()
    {
        //storeCash = 13400;
        AddCashToInventory(13400);
    }
}
