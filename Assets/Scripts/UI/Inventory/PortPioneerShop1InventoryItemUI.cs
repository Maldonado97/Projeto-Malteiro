using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortPioneerShop1InventoryItemUI : DockShopOwnInventoryItemUI
{
    protected override void SetParentSingletonReferences()
    {
        shopScreenManager = PortPioneerShop1ScreenManager.instance;
        parentInventoryManager = PortPioneerShop1InventoryManager.instance;
    }
}
