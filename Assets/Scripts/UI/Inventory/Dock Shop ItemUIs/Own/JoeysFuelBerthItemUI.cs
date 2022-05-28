using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoeysFuelBerthItemUI : DockShopInventoryItemUI
{
    protected override void SetParentSingletonReferences()
    {
        shopScreenManager = JoeysFuelBerthScreenManager.instance;
        shopInventoryManager = JoeysFuelBerthInventoryManager.instance;
    }
    protected override void SetItemValueModifications()
    {
        baseValueModification = 0;
        //DEFAULT VALUE MODIFICATIONS
        foreach (string itemName in GameItemDictionary.instance.gameItemNames)
        {
            itemValueModifications.Add(itemName, 0);
        }
        //CUSTOM VALUE MODIFICATIONS
        float fuelDrum = .1f;

        itemValueModifications["Fuel Drum"] = fuelDrum;
    }
}
