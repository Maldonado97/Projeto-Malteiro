using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BudgetRepairsScreenManager : DryDockScreenManager
{
    public static BudgetRepairsScreenManager instance;
    public override void SetInstance()
    {
        instance = this;
    }
}
