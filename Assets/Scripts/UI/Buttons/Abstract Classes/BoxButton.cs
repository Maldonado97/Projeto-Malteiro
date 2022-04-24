using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class BoxButton : CustomButton
{
    [SerializeField] protected Color defaultTextColor;
    [SerializeField] protected Color defaultBoxColor;
    [SerializeField] protected Color highlightedTextColor;
    [SerializeField] protected Color highlightedBoxColor;
    [SerializeField] protected GameObject buttonText;
    [SerializeField] protected GameObject box;

    protected TextMeshProUGUI buttonTextTMPro;
    protected Image boxImage;

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
}
