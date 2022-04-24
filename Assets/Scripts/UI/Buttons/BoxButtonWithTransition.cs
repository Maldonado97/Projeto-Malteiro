using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class BoxButtonWithTransition : BoxButton
{
    [Header("Transitions")]
    [SerializeField] GameObject screenToOpen;
    [SerializeField] GameObject screenToClose;

    //USER INTERFACE METHODS
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (screenToOpen != null)
        {
            screenToOpen?.SetActive(true);
        }
        if (screenToClose != null)
        {
            screenToClose?.SetActive(false);
        }
        UnhighlightButton();
    }
}
