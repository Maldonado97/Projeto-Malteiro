using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;
    public UnityEvent onPointerClick;
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick.Invoke();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown.Invoke();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp.Invoke();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke();
    }
}
