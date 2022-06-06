using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public abstract class DryDockServiceOption : BoxButton
{
    [SerializeField] protected DryDockScreenManager dryDockScreenManager;
    [HideInInspector] public int orderInList;
    public TextMeshProUGUI serviceValueText;
    [HideInInspector] public string serviceName;
    [HideInInspector] public string serviceDescription;
    [HideInInspector] public float serviceValue;

    public event Action <DryDockServiceOption> onServiceClicked;
    public void Start()
    {
        orderInList = transform.GetSiblingIndex();
        SetDryDockScreenManager();
        dryDockScreenManager.IntializeServiceOption(this);
        displayServiceOptionInformation();
    }
    public abstract void SetDryDockScreenManager();
    public void displayServiceOptionInformation()
    {
        buttonTextTMPro.text = $"{serviceName}";
        serviceValueText.text = $"{serviceValue}";
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onServiceClicked?.Invoke(this);
    }
}
