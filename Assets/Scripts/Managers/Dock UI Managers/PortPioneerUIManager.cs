using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortPioneerUIManager : DockUIManager
{
    protected override void SetInstance()
    {
        instance = this;
    }
}
