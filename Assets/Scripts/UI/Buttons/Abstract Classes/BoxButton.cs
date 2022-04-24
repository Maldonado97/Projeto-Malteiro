using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public abstract class BoxButton : CustomButton
{
    [SerializeField] Color defaultTextColor;
    [SerializeField] Color defaultBoxColor;
    [SerializeField] Color highlightedTextColor;
    [SerializeField] Color highlightedBoxColor;
    [SerializeField] GameObject buttonText;
    [SerializeField] GameObject box;

    private TextMeshProUGUI buttonTextTMPro;
    private Image boxImage;

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
        base.OnPointerEnter(eventData);

        HighlightButton();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        UnhighlightButton();
    }
}
