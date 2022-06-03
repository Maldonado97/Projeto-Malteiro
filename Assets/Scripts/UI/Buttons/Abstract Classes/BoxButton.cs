using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class BoxButton : CustomButton
{
    public Color defaultTextColor;
    public Color defaultBoxColor;
    public Color highlightedTextColor;
    public Color highlightedBoxColor;
    public Color selectedTextColor;
    public Color selectedBoxColor;
    public GameObject buttonText;
    public GameObject box;

    [HideInInspector] public bool selected = false;
    [HideInInspector] public TextMeshProUGUI buttonTextTMPro;
    [HideInInspector] public Image boxImage;
    private bool mouseOnButton = false;

    public void Start()
    {
        buttonTextTMPro = buttonText.GetComponent<TextMeshProUGUI>();
        boxImage = box.GetComponent<Image>();
    }
    public void HighlightButton()
    {
        buttonTextTMPro.color = highlightedTextColor;
        boxImage.color = highlightedBoxColor;
    }
    public void UnhighlightButton()
    {
        buttonTextTMPro.color = defaultTextColor;
        boxImage.color = defaultBoxColor;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        mouseOnButton = true;
        if (!selected)
        {
            HighlightButton();
        }
        base.OnPointerEnter(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        mouseOnButton = false;
        if (!selected)
        {
            UnhighlightButton();
        }
        base.OnPointerExit(eventData);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!selected)
        {
            UnhighlightButton();
        }
        base.OnPointerDown(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (mouseOnButton)
        {
            HighlightButton();
        }
        base.OnPointerUp(eventData);
    }
}
