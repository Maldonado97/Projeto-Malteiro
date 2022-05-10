using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopeCityUIManager : DockUIManager
{
    protected override void SetInstance()
    {
        instance = this;
    }
}
