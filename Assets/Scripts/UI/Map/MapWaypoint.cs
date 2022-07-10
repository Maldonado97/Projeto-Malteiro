using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapWaypoint : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    MapControl mapControl;
    [SerializeField] GameObject line;
    RectTransform wayPointRectTransform;
    [HideInInspector] public Vector3 waypointPosition;
    Vector3 previousWaypointPosition;
    [HideInInspector] public int listIndex;
    //LINE

    public event Action onPreviousWaypointChanged;
    public event Action onDestroy;
    public event Action onBecameFirstWaypoint;
    private void Start()
    {
        mapControl = MapControl.instance;
        line = mapControl.linePrefab;
        mapControl.mapWaypoints.Add(this);
        wayPointRectTransform = gameObject.GetComponent<RectTransform>();
        GetWaypointPosition();
        GetListIndex();
        CorrectHierarchy();

        if(listIndex > 0)
        {
            GetPreviousWaypointPosition();
            DrawLine(); //Makes line connecting this waypoint to previous waypoint
        }

        mapControl.onMapPositionChanged += GetWaypointPosition;
        mapControl.onWaypointRemoved += OnWaypointRemoved;
    }
    public void CorrectHierarchy()
    {
        RectTransform playerIndicatorRectTransform = mapControl.playerIndicator.GetComponent<RectTransform>();
        RectTransform previewLineRectTransform = mapControl.previewLine.GetComponent<RectTransform>();
        RectTransform pathInformationBoxRectTransform = mapControl.pathInformationBox.GetComponent<RectTransform>();
        playerIndicatorRectTransform.SetAsLastSibling();
        previewLineRectTransform.SetAsLastSibling();
        pathInformationBoxRectTransform.SetAsLastSibling();
    }
    private void GetWaypointPosition()
    {
        waypointPosition = wayPointRectTransform.position * mapControl.canvas.scaleFactor;
    }
    private void GetListIndex()
    {
        listIndex = mapControl.mapWaypoints.IndexOf(this);
    }
    private void GetPreviousWaypointPosition()
    {
        previousWaypointPosition = mapControl.mapWaypoints[listIndex - 1].waypointPosition;
    }
    private void DrawLine()
    {
        Instantiate(line, waypointPosition, transform.rotation, mapControl.mapTransform);
    }
    public Vector3 GetLineRotation()
    {
        float lineZRotation = -GetPointBearing(waypointPosition, previousWaypointPosition);
        Vector3 lineRotation = new Vector3(0, 0, lineZRotation);

        return lineRotation;
    }
    public float GetLineHeight()
    {
        float lineHeight;
        lineHeight = Vector3.Distance(waypointPosition, previousWaypointPosition) / mapControl.mapTransform.localScale.x;
        //lineRectTransform.sizeDelta = new Vector2(lineWidth, lineHeight);

        return lineHeight;
    }
    float GetPointBearing(Vector3 origin, Vector3 endPoint)
    {
        float targetRawBearing;
        float mouseBearing;
        Vector2 bearingVector;

        bearingVector = origin - endPoint;
        targetRawBearing = Mathf.Atan2(bearingVector.y, bearingVector.x) * Mathf.Rad2Deg;

        if (targetRawBearing < 0)
        {
            targetRawBearing += 360;
        }

        mouseBearing = 360 - targetRawBearing - 90;

        if (mouseBearing < 0)
        {
            mouseBearing += 360;
        }

        return mouseBearing;
    }
    //EVENTS
    public void OnWaypointRemoved(int removedWaypointListIndex)
    {
        if(removedWaypointListIndex < listIndex)
        {
            listIndex -= 1;
            if(listIndex > 0)
            {
                GetPreviousWaypointPosition();
                onPreviousWaypointChanged?.Invoke();
            }
            else
            {
                onBecameFirstWaypoint?.Invoke();
            }
        }
    }
    //POINTER EVENTS
    public void OnPointerClick(PointerEventData eventData)
    {
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //wayPointRectTransform.localScale += new Vector3(1, 1, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //wayPointRectTransform.localScale -= new Vector3(1, 1, 0);
    }
    private void OnDestroy()
    {
        mapControl.mapWaypoints.Remove(this);
        mapControl.onMapPositionChanged -= GetWaypointPosition;
        mapControl.OnWaypointRemoved(listIndex);
        onDestroy?.Invoke();
    }
}
