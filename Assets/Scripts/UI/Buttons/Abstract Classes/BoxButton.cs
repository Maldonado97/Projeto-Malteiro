using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class BoxButton : CustomButton
{
    [Header("Button Box")]
    public GameObject box;
    public Color defaultBoxColor;
    public Color highlightedBoxColor;
    public Color selectedBoxColor;
    [Header("Button Text")]
    [Tooltip("If the button has any text.")]
    public GameObject buttonText;
    public Color defaultTextColor;
    public Color highlightedTextColor;
    public Color selectedTextColor;

    [HideInInspector] public bool selected = false;
    [HideInInspector] public TextMeshProUGUI buttonTextTMPro;
    [HideInInspector] public Image boxImage;
    protected bool mouseOnButton = false;

    bool buttonHasText = false;

    public event Action onButtonClicked;

    public void Awake()
    {
        if(buttonText != null)
        {
            buttonTextTMPro = buttonText.GetComponent<TextMeshProUGUI>();
            buttonHasText = true;
        }

        boxImage = box.GetComponent<Image>();
    }
    public void HighlightButton()
    {
        if (buttonHasText)
        {
            buttonTextTMPro.color = highlightedTextColor;
        }
        boxImage.color = highlightedBoxColor;
    }
    public void SelectButton()
    {
        selected = true;
        if (buttonHasText)
        {
            buttonTextTMPro.color = selectedTextColor;
        }
        boxImage.color = selectedBoxColor;
    }
    public void DeselectButton()
    {
        selected = false;
        UnhighlightButton();
    }
    public void UnhighlightButton()
    {
        if (buttonHasText)
        {
            buttonTextTMPro.color = defaultTextColor;
        }
        boxImage.color = defaultBoxColor;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        onButtonClicked?.Invoke();
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
