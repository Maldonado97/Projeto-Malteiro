using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopeCityShop1InventoryItemUI : DockShopInventoryItemUI
{
    protected override void SetParentSingletonReferences()
    {
        shopScreenManager = HopeCityShop1ScreenManager.instance;
        shopInventoryManager = HopeCityShop1InventoryManager.instance;
    }
}
