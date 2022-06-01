using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoeysFuelBerthScreenManager : GeneralDockShopScreenManager
{
    public static JoeysFuelBerthScreenManager instance;

    public override void Start()
    {
        base.Start();
        customPlayerSubInventoryItemTypes.Add("Fuel");
        customPlayerSubInventoryItemTypes.Add("Contraband");
        ChangePlayerInventoryFilter("Custom");
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
