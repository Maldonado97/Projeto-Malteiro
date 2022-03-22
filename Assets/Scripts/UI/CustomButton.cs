using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;
    public UnityEvent onPointerClick;
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick.Invoke();
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
