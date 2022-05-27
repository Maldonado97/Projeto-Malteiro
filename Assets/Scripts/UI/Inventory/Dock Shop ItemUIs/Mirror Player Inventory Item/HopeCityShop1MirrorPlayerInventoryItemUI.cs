using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopeCityShop1MirrorPlayerInventoryItemUI : DockShopMirrorPlayerInventoryItemUI
{
    protected override void SetParentSingletonReferences()
    {
        shopScreenManager = HopeCityShop1ScreenManager.instance;
        shopInventoryManager = HopeCityShop1InventoryManager.instance;
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
        float boxOfSteaks = .30f;
        float cake = .30f;
        float contraband = .40f;
        float emptyCrate = -.125f;
        float medicineBox = -.125f;
        float scrapMetal = .25f;

        itemValueModifications["Box of Steaks"] = boxOfSteaks;
        itemValueModifications["Cake"] = cake;
        itemValueModifications["Contraband"] = contraband;
        itemValueModifications["Empty Crate"] = emptyCrate;
        itemValueModifications["Medicine Box"] = medicineBox;
        itemValueModifications["Scrap Metal"] = scrapMetal;
    }
}
