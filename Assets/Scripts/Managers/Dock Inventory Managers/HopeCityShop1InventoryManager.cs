using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopeCityShop1InventoryManager : GeneralDockInventoryManager
{
    public static HopeCityShop1InventoryManager instance;
    private void Awake()
    {
        instance = this;
        storeCash = 13400;
    }
}
