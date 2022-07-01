using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapWaypoint : MonoBehaviour
{
    MapControl mapControl;
    [SerializeField] GameObject line;
    RectTransform wayPointRectTransform;
    [HideInInspector] public Vector3 waypointPosition;
    Vector3 previousWaypointPosition;
    int listIndex;
    //LINE

    private void Start()
    {
        mapControl = MapControl.instance;
        line = mapControl.linePrefab;
        mapControl.mapWaypoints.Add(this);
        wayPointRectTransform = gameObject.GetComponent<RectTransform>();
        GetWaypointPosition();
        GetListIndex();

        if(listIndex > 0)
        {
            DrawLine();
        }

        mapControl.onMapPositionChanged += GetWaypointPosition;
    }
    private void GetWaypointPosition()
    {
        waypointPosition = wayPointRectTransform.position * mapControl.canvas.scaleFactor;
    }
    private void GetListIndex()
    {
        listIndex = mapControl.mapWaypoints.IndexOf(this);
    }
    private void DrawLine()
    {
        previousWaypointPosition = mapControl.mapWaypoints[listIndex - 1].waypointPosition;
        Instantiate(line, waypointPosition, transform.rotation, wayPointRectTransform);
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
    private void OnDestroy()
    {
        mapControl.onMapPositionChanged -= GetWaypointPosition;
    }
}
