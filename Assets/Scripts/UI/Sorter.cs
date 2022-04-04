using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sorter : CustomButton
{
    [SerializeField] GameObject dropDownBox;
    [SerializeField] Color defaultColor;
    [SerializeField] Color highlightedColor;
    private Image sorterImage;

    public void Start()
    {
        sorterImage = gameObject.GetComponent<Image>();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        sorterImage.color = highlightedColor;
        dropDownBox.SetActive(true);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        sorterImage.color = defaultColor;
        dropDownBox.SetActive(false);
    }
}
