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
        if (!selected)
        {
            UnhighlightButton();
        }
        base.OnPointerClick(eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
        {
            HighlightButton();
        }
        base.OnPointerEnter(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            UnhighlightButton();
        }
        base.OnPointerExit(eventData);
    }
}
