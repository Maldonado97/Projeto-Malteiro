using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class DockMenuButton : CustomButton
{
    [SerializeField] Color defaultColor;
    [SerializeField] Color highlightedColor;
    [SerializeField] GameObject buttonText;
    [SerializeField] GameObject underline;
    [Header("Transitions")]
    [SerializeField] GameObject screenToOpen;
    [SerializeField] GameObject screenToClose;

    private TextMeshProUGUI buttonTextTMPro;
    private Image underlineImage;

    public void Start()
    {
        buttonTextTMPro = buttonText.GetComponent<TextMeshProUGUI>();
        underlineImage = underline.GetComponent<Image>();

        //throw new System.NotImplementedException();
        //throw new System.NullReferenceException();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if(screenToOpen != null)
        {
            screenToOpen?.SetActive(true);
        }
        if(screenToClose != null)
        {
            screenToClose?.SetActive(false);
        }
        UnhighlightButton();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        HighlightButton();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        UnhighlightButton();
    }
    public void HighlightButton()
    {
        buttonTextTMPro.color = highlightedColor;
        underlineImage.color = highlightedColor;
    }
    public void UnhighlightButton()
    {
        buttonTextTMPro.color = defaultColor;
        underlineImage.color = defaultColor;
    }
    public void ReleaseShip()
    {
        PlayerControl.instance.UndockShip();
    }
}
