using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLine : MonoBehaviour
{
    MapControl mapControl;
    RectTransform lineRectTransform;
    MapWaypoint parentWaypoint;
    float lineHeight;
    float lineWidth = 2;
    Vector3 lineRotation;
    private void Start()
    {
        mapControl = MapControl.instance;
        lineRectTransform = gameObject.GetComponent<RectTransform>();
        parentWaypoint = mapControl.mapWaypoints[mapControl.mapWaypoints.Count - 1];
        lineHeight = parentWaypoint.GetLineHeight();
        lineRotation = parentWaypoint.GetLineRotation();

        lineRectTransform.sizeDelta = new Vector2(lineWidth, lineHeight);
        lineRectTransform.eulerAngles = lineRotation;
    }
}
