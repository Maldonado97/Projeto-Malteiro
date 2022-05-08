using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopeCityShop1ScreenManager : GeneralDockShopScreenManager
{
    public static HopeCityShop1ScreenManager instance;

    public override void SetInstance()
    {
        instance = this;
    }
    public override void SetOwnInventoryReference()
    {
        ownInventory = HopeCityShop1InventoryManager.instance;
    }
    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        //ERROR: PortPioneerShop1InventoryManager not being recognized for some odd reason..
        //ERROR CAUSE: Inventory manager was loading after this  script, making it impossible to find it.
        //SOLUTION: changed start method on inventory manager to awake, so it would load before this script.
        HopeCityShop1InventoryManager.instance.onInventoryItemAdded += CreateItemUI;
    }
}
