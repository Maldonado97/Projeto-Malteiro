using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickFixScreenManager : DryDockScreenManager
{
    public static QuickFixScreenManager instance;
    public override void SetInstance()
    {
        instance = this;
    }
}