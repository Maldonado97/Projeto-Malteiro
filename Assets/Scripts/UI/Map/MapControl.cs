using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapControl : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public static MapControl instance;

    public Canvas canvas;
    [SerializeField] GameObject border;
    public GameObject playerIndicator;
    [Header("Scale Calculation")]
    [SerializeField] Transform realPoint1;
    [SerializeField] Transform realPoint2;
    [SerializeField] RectTransform mapPoint1;
    [SerializeField] RectTransform mapPoint2;
    [Header("Route Plotting")]
    private List<BoxButton> navTools = new List<BoxButton>();
    [SerializeField] BoxButton plotRouteButton;
    [SerializeField] BoxButton editRouteButton;
    public GameObject previewLine;
    private Image previewLineImage;
    [SerializeField] GameObject waypointPrefab;
    public GameObject linePrefab;
    [HideInInspector] public bool editingWaypoints = false;
    bool doingSomethingRandom = false;

    bool draggingMap = false;
    [HideInInspector] public RectTransform mapTransform;
    RectTransform borderRectTransform;
    GameObject player;
    private float xScale;
    private float yScale;
    private float xOffset;
    private float yOffset;
    [HideInInspector] public List<MapWaypoint> mapWaypoints = new List<MapWaypoint>();

    public event Action onPlayerClick;
    public event Action onMapPositionChanged;
    public event Action<int> onWaypointRemoved;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        mapTransform = gameObject.GetComponent<RectTransform>();
        borderRectTransform = border.GetComponent<RectTransform>();
        player = PlayerControl.instance.gameObject;
        navTools.Add(plotRouteButton);
        navTools.Add(editRouteButton);
        GetMapScaleFactor();
        previewLineImage = previewLine.GetComponent<Image>();
        previewLineImage.enabled = false;
        //previewLine.SetActive(false);
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

        xScale = mapDistanceX / realDistanceX;
        yScale = mapDistanceY / realDistanceY;
        xOffset = mapPoint1.anchoredPosition.x - (realPoint1.position.x * xScale);
        yOffset = mapPoint1.anchoredPosition.y - (realPoint1.position.y * yScale);
    }
    private void SetMapItemPosition(GameObject realItem, GameObject mapItem)
    {
        float xPos = (realItem.transform.position.x * xScale) + xOffset;
        float yPos = (realItem.transform.position.y * yScale) + yOffset;

        RectTransform mapItemRectTransform = mapItem.GetComponent<RectTransform>();
        mapItemRectTransform.anchoredPosition = new Vector3(xPos, yPos);
        mapItemRectTransform.rotation = realItem.transform.rotation;
        //Debug.Log($"Map item position = {xPos}, {yPos}");
    }
    public void OnWaypointRemoved(int waypointListIndex)
    {
        if (waypointListIndex == (mapWaypoints.Count))
        {
            if(mapWaypoints.Count > 0)
            {
                previewLine.transform.position = mapWaypoints[mapWaypoints.Count - 1].waypointPosition;
            }
            else
            {
                previewLineImage.enabled = false;
            }
        }
        onWaypointRemoved?.Invoke(waypointListIndex);
    }
    public void OnNavToolClicked(BoxButton navTool)
    {
        if (navTool.selected)
        {
            if (navTool == plotRouteButton)
            {
                editingWaypoints = false;
                //Debug.Log("Deselected Plot Route button");
            }
            if (navTool == editRouteButton)
            {
                doingSomethingRandom = false;
                //Debug.Log("Deselected Edit Route button");
            }
            navTool.DeselectButton();
        }
        else
        {
            if (navTool == plotRouteButton)
            {
                editingWaypoints = true;
                doingSomethingRandom = false;
                //Debug.Log("Selected Plot Route button");
            }
            if (navTool == editRouteButton)
            {
                doingSomethingRandom = true;
                editingWaypoints = false;
                //Debug.Log("Selected Do Something Random button");
            }
            DeselectAllNavTools();
            navTool.SelectButton();
        }
        if (editingWaypoints && mapWaypoints.Count > 0)
        {
            //previewLine.SetActive(true);
            previewLineImage.enabled = true;
        }
        else
        {
            //previewLine.SetActive(false);
            previewLineImage.enabled = false;
        }
    }
    public void DeselectAllNavTools()
    {
        foreach(BoxButton navTool in navTools)
        {
            navTool.DeselectButton();
        }
    }
    //POINTER EVENTS
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

        onMapPositionChanged?.Invoke();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        draggingMap = true;
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
        draggingMap = false;
        onMapPositionChanged?.Invoke();
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!draggingMap && editingWaypoints && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            Vector3 mousePosition = eventData.position * canvas.scaleFactor;
            //Debug.Log($"{mousePosition}, {Input.mousePosition * canvas.scaleFactor}");
            //Vector3 mouseRealtivePosition = (mousePosition - mapTransform.position) / mapTransform.localScale.x;

            Instantiate(waypointPrefab, mousePosition, mapTransform.rotation, mapTransform);
            previewLine.transform.position = mousePosition;
            //previewLine.SetActive(true);
            previewLineImage.enabled = true;
            //Instantiate(line, mousePosition, mapTransform.rotation, mapTransform);
            onPlayerClick?.Invoke();
        }
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
