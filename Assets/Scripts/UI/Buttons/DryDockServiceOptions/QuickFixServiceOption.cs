using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickFixServiceOption : DryDockServiceOption
{
    public override void SetDryDockScreenManager()
    {
        dryDockScreenManager = QuickFixScreenManager.instance;
    }
}
