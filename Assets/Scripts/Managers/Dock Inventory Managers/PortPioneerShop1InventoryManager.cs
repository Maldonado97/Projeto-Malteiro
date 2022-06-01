using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortPioneerShop1InventoryManager : GeneralDockInventoryManager
{
    public static PortPioneerShop1InventoryManager instance;

    protected override void SetInstance()
    {
        instance = this;
    }

    protected override void SetStartingCash()
    {
        storeCash = 8200;
    }
}
