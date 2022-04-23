using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PortPioneerShop1ScreenManager : GeneralDockShopScreenManager
{
    public static PortPioneerShop1ScreenManager instance;

    public override void SetInstance()
    {
        instance = this;
    }
    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        //ERROR: PortPioneerShop1InventoryManager not being recognized for some odd reason..
        //ERROR CAUSE: Inventory manager was loading after this  script, making it impossible to find it.
        //SOLUTION: changed start method on inventory manager to awake, so it would load before this script.
        PortPioneerShop1InventoryManager.instance.onInventoryItemAdded += CreateItemUI;
    }
}
