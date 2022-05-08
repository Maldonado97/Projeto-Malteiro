using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortPioneerShop1InventoryManager : GeneralDockInventoryManager
{
    public static PortPioneerShop1InventoryManager instance;
    public void Awake() //Should be set to awake, otherwise screen manager can't find its instance
    {
        instance = this;
        storeCash = 8200;
    }
}
