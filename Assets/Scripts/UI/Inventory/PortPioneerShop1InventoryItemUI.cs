using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortPioneerShop1InventoryItemUI : DockShopInventoryItemUI
{
    protected override void SetParentSingletonReferences()
    {
        shopScreenManager = PortPioneerShop1ScreenManager.instance;
        shopInventoryManager = PortPioneerShop1InventoryManager.instance;
    }
}
