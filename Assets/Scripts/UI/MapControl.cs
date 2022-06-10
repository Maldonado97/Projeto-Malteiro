using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapControl : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject border;
    [SerializeField] GameObject playerIndicator;
    [SerializeField] Transform realPoint1;
    [SerializeField] Transform realPoint2;
    [SerializeField] RectTransform mapPoint1;
    [SerializeField] RectTransform mapPoint2;

    RectTransform mapTransform;
    RectTransform borderRectTransform;
    GameObject player;
    private float xScaleFactor;
    private float yScaleFactor;
    private float xOffset;
    private float yOffset;

    //public UnityEvent onPointerEnter;
    //public UnityEvent onPointerExit;
    //public UnityEvent onPointerClick;
    //public UnityEvent onPointerDown;
    //public UnityEvent onPointerUp;
    //public UnityEvent onScroll;
    //public UnityEvent onDrag;
    private void Start()
    {
        mapTransform = gameObject.GetComponent<RectTransform>();
        borderRectTransform = border.GetComponent<RectTransform>();
        player = PlayerControl.instance.gameObject;
        GetMapScaleFactor();
    }
    private void Update()
    {
        SetMapItemPosition(player, playerIndicator);
    }
    private void GetMapScaleFactor()
    {
        float mapDistanceX = Mathf.Abs(mapPoint2.anchoredPosition.x - mapPoint1.anchoredPosition.x);
        float mapDistanceY = Mathf.Abs(mapPoint2.anchoredPosition.y - mapPoint1.anchoredPosition.y);
        float realDistanceX = Mathf.Abs(realPoint2.position.x - realPoint1.position.x);
        float realDistanceY = Mathf.Abs(realPoint2.position.y - realPoint1.position.y);

        xScaleFactor = mapDistanceX / realDistanceX;
        yScaleFactor = mapDistanceY / realDistanceY;
        xOffset = mapPoint1.anchoredPosition.x - (realPoint1.position.x * xScaleFactor);
        yOffset = mapPoint1.anchoredPosition.y - (realPoint1.position.y * yScaleFactor);
    }
    private void SetMapItemPosition(GameObject realItem, GameObject mapItem)
    {
        float xPos = (realItem.transform.position.x * xScaleFactor) + xOffset;
        float yPos = (realItem.transform.position.y * yScaleFactor) + yOffset;

        RectTransform mapItemRectTransform = mapItem.GetComponent<RectTransform>();
        mapItemRectTransform.anchoredPosition = new Vector3(xPos, yPos);
        mapItemRectTransform.rotation = realItem.transform.rotation;
        //Debug.Log($"Map item position = {xPos}, {yPos}");
    }
    public void OnScroll(PointerEventData eventData)
    {
        //onScroll?.Invoke();
        var xScale = mapTransform.localScale.x;
        var yScale = mapTransform.localScale.y;
        if(xScale > 1 && xScale < 9)
        {
            xScale += Input.mouseScrollDelta.y;
            yScale += Input.mouseScrollDelta.y;
        }
        if(xScale <= 1 && Input.mouseScrollDelta.y > 0)
        {
            xScale += Input.mouseScrollDelta.y;
            yScale += Input.mouseScrollDelta.y;
        }
        if(xScale >= 9 && Input.mouseScrollDelta.y < 0)
        {
            xScale += Input.mouseScrollDelta.y;
            yScale += Input.mouseScrollDelta.y;
        }
        if(xScale < 1)
        {
            xScale = 1;
            yScale = 1;
        }
        if(xScale > 9)
        {
            xScale = 9;
            yScale = 9;
        }
        mapTransform.localScale = new Vector3(xScale, yScale);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
    }
    public void OnDrag(PointerEventData eventData)
    {
        float xBound;
        float yBound;
        var yPos = mapTransform.anchoredPosition.y;
        var xPos = mapTransform.anchoredPosition.x;

        if (mapTransform.rect.width * mapTransform.localScale.x <= borderRectTransform.rect.width)
        {
            xBound = (borderRectTransform.rect.width / 2) - (mapTransform.rect.width * mapTransform.localScale.x) / 2;
        }
        else
        {
            xBound =((mapTransform.rect.width * mapTransform.localScale.x) - borderRectTransform.rect.width) / 2;
        }

        if (mapTransform.rect.height * mapTransform.localScale.y <= borderRectTransform.rect.height)
        {
            yBound = (borderRectTransform.rect.height / 2) - ((mapTransform.rect.height * mapTransform.localScale.y) / 2);
        }
        else
        {
            //Debug.Log($"x = {mapTransform.rect.height * mapTransform.localScale.y}, y = {borderRectTransform.rect.height}");
            yBound = ((mapTransform.rect.height * mapTransform.localScale.y) - borderRectTransform.rect.height) / 2;
        }
        //Debug.Log($"{xBound}");

        if (mapTransform.anchoredPosition.x <= xBound && mapTransform.anchoredPosition.x >= -xBound)
        {
            xPos += eventData.delta.x / canvas.scaleFactor;
        }
        else if(mapTransform.anchoredPosition.x > xBound && eventData.delta.x < 0)
        {
            //xPos += eventData.delta.x / canvas.scaleFactor;
        }
        else if (mapTransform.anchoredPosition.x < -xBound && eventData.delta.x > 0)
        {
            //xPos += eventData.delta.x / canvas.scaleFactor;
        }

        if (mapTransform.anchoredPosition.y <= yBound && mapTransform.anchoredPosition.y >= -yBound)
        {
            yPos += eventData.delta.y / canvas.scaleFactor;
            //Debug.Log($"Moving y, yPos: {yPos}");
        }
        else if(mapTransform.anchoredPosition.y > yBound && eventData.delta.y < 0)
        {
            //yPos += eventData.delta.y / canvas.scaleFactor;
        }
        else if (mapTransform.anchoredPosition.y < -yBound && eventData.delta.y > 0)
        {
            //yPos += eventData.delta.y / canvas.scaleFactor;
        }

        if (xPos < -xBound)
        {
            xPos = -xBound;
        }
        if (xPos > xBound)
        {
            xPos = xBound;
        }
        if (yPos < -yBound)
        {
            yPos = -yBound;
        }
        if (yPos > yBound)
        {
            yPos = yBound;
        }

        //Debug.Log($"xPos: {xPos}, xBound: {xBound}");
        //Debug.Log($"yPos: {yPos}, yBound: {yBound}");
        mapTransform.anchoredPosition = new Vector2(xPos, yPos);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
    }
}