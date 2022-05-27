using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortPioneerShop1MirrorPlayerInventoryItemUI : DockShopMirrorPlayerInventoryItemUI
{
    protected override void SetParentSingletonReferences()
    {
        shopScreenManager = PortPioneerShop1ScreenManager.instance;
        shopInventoryManager = PortPioneerShop1InventoryManager.instance;
    }
    protected override void SetItemValueModifications()
    {
        baseValueModification = -.125f;
        //DEFAULT VALUE MODIFICATIONS
        foreach (string itemName in GameItemDictionary.instance.gameItemNames)
        {
            itemValueModifications.Add(itemName, 0);
        }
        //CUSTOM VALUE MODIFICATIONS
        float boxOfSteaks = -.125f;
        float cake = -.125f;
        float contraband = 0f;
        float emptyCrate = 0f;
        float medicineBox = .40f;
        float scrapMetal = .30f;

        itemValueModifications["Box of Steaks"] = boxOfSteaks;
        itemValueModifications["Cake"] = cake;
        itemValueModifications["Contraband"] = contraband;
        itemValueModifications["Empty Crate"] = emptyCrate;
        itemValueModifications["Medicine Box"] = medicineBox;
        itemValueModifications["Scrap Metal"] = scrapMetal;
    }
}
