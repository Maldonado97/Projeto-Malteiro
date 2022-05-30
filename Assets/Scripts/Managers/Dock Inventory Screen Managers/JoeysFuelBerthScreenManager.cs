using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoeysFuelBerthScreenManager : GeneralDockShopScreenManager
{
    public static JoeysFuelBerthScreenManager instance;

    public override void Start()
    {
        base.Start();
        activePlayerInventory = PlayerInventoryManager.instance.fuelItemsInInventory;
    }
    public override void SetInstance()
    {
        instance = this;
    }
    public override void SetOwnInventoryReference()
    {
        ownInventory = JoeysFuelBerthInventoryManager.instance;
    }
}
