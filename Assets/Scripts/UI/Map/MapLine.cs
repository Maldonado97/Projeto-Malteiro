using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    MapControl mapControl;
    RectTransform lineRectTransform;
    Image lineImage;
    MapWaypoint parentWaypoint;
    int parentWaypointListIndex;
    float lineWidth = 2;
    private void Start()
    {
        mapControl = MapControl.instance;
        lineRectTransform = gameObject.GetComponent<RectTransform>();
        lineRectTransform.SetAsFirstSibling();
        parentWaypoint = mapControl.mapWaypoints[mapControl.mapWaypoints.Count - 1];
        lineImage = gameObject.GetComponent<Image>();
        parentWaypointListIndex = parentWaypoint.listIndex;
        GetLineHeightAndRotation();

        parentWaypoint.onPreviousWaypointChanged += OnPreviousWaypointChanged;
        parentWaypoint.onDestroy += DestroyLine;
        parentWaypoint.onBecameFirstWaypoint += DestroyLine;
    }
    private void GetLineHeightAndRotation()
    {
        float lineHeight = parentWaypoint.GetLineHeight();
        Vector3 lineRotation = parentWaypoint.GetLineRotation();

        lineRectTransform.sizeDelta = new Vector2(lineWidth, lineHeight);
        lineRectTransform.eulerAngles = lineRotation;
    }
    private void OnPreviousWaypointChanged()
    {
        parentWaypointListIndex = parentWaypoint.listIndex;
        GetLineHeightAndRotation();
    }
    private void DestroyLine()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        parentWaypoint.onPreviousWaypointChanged -= OnPreviousWaypointChanged;
        parentWaypoint.onDestroy -= DestroyLine;
        parentWaypoint.onBecameFirstWaypoint -= DestroyLine;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
    }
    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
