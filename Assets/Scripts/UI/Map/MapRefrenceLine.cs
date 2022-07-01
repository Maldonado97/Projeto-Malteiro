using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapRefrenceLine : MonoBehaviour
{
    Vector3 basePosition;
    Vector3 mousePosition;
    RectTransform lineRectTransform;
    float lineWidth = 2;
    float lineHeight;
    bool editingLine = true;
    private void Start()
    {
        lineRectTransform = gameObject.GetComponent<RectTransform>();

        //MapControl.instance.onPlayerClick += StopEditingLine;
    }
    private void Update()
    {
        if (editingLine)
        {
            mousePosition = Input.mousePosition * MapControl.instance.canvas.scaleFactor;
            basePosition = lineRectTransform.position * MapControl.instance.canvas.scaleFactor;
            SetLineRotation();
            SetLineHeight();
        }
    }
    float GetMouseBearing(Vector3 origin, Vector3 mousePosition)
    {
        float targetRawBearing;
        float mouseBearing;
        Vector2 bearingVector;

        bearingVector = origin - mousePosition;
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
    private void SetLineRotation()
    {
        float lineZRotation = -GetMouseBearing(basePosition, mousePosition);
        lineRectTransform.eulerAngles = new Vector3(0, 0, lineZRotation);
    }
    private void SetLineHeight()
    {
        lineHeight = Vector3.Distance(basePosition, mousePosition) / MapControl.instance.mapTransform.localScale.x;
        lineRectTransform.sizeDelta = new Vector2(lineWidth, lineHeight);
    }
    private void StopEditingLine()
    {
        editingLine = false;
    }
    private void OnDestroy()
    {
        //MapControl.instance.onPlayerClick -= StopEditingLine;
    }
}
