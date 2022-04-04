using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SorterOption : CustomButton
{
    [SerializeField] string sortMode;
    [Header("Button Options")]
    [SerializeField] Color defaultColor;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color clickedColor;
    private Image optionImage;
    private bool mouseOverOption;
    // Start is called before the first frame update
    void Start()
    {
        optionImage = gameObject.GetComponent<Image>();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        PlayerInventoryManager.instance.ChangeSortMode(sortMode);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        optionImage.color = clickedColor;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (mouseOverOption)
        {
            optionImage.color = highlightedColor;
        }
        else
        {
            optionImage.color = defaultColor;
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        optionImage.color = highlightedColor;
        mouseOverOption = true;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        optionImage.color = defaultColor;
        mouseOverOption = false;
    }
}
