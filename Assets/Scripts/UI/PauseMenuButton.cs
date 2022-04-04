using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class PauseMenuButton :MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject underline;
    private Image underlineImage;
    [SerializeField] GameObject buttonTextBox;
    private TextMeshProUGUI buttonTextBoxText;
    [SerializeField] Color defaultUnderlineColor;
    [SerializeField] Color highlightedUnderlineColor;
    [SerializeField] Color defaultTextColor;
    [SerializeField] Color highlightedTextColor;


    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;
    public UnityEvent onPointerClick;

    public void Start()
    {
        underlineImage = underline.GetComponent<Image>();
        buttonTextBoxText = buttonTextBox.GetComponent<TextMeshProUGUI>();
        underlineImage.color = defaultUnderlineColor;
        buttonTextBoxText.color = defaultTextColor;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke();
    }

    public void TestEvent()
    {
        Debug.Log("Hey!");
    }
    public void highlightButton()
    {
        underlineImage.color = highlightedUnderlineColor;
        buttonTextBoxText.color = highlightedTextColor;
    }
    public void unhighlightButton()
    {
        underlineImage.color = defaultUnderlineColor;
        buttonTextBoxText.color = defaultTextColor;
    }
}
