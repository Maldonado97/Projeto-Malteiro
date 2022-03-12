using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Tooltip("Determines what the camera will follow (Should be player).")]
    [SerializeField] GameObject target;
    [Header("Zoom Settings")]
    [SerializeField] float defaultZoom;
    [Header("Deadzone settings")]
    [Tooltip("Determines max distance between the target and the camera before the camera begins" +
        " moving.")]
    [SerializeField] float positionDeadZoneMax; //Ideal = 2
    [Tooltip("Determines at which distance from the target the camera should stop moving.")]
    [SerializeField] float positionDeadZoneMin; //Ideal = 0.2
    [SerializeField] float rotationDeadZoneMax;
    [SerializeField] float rotationDeadZoneMin;
    [Header("Lerp settings")]
    [Tooltip("Determines how fast the camera will follow the player.")]
    [SerializeField] float positionLerpAlfa; //Ideal = 0.015f
    [Tooltip("Determines how fast the camera will ajust to the player's rotation.")]
    [SerializeField] float rotationLerpAlfa; //Ideal = 0.009f

    private Vector3 cameraPos;
    private Camera cameraComponent;
    private float targetXPos;
    private float targetYPos;
    private bool positionDeadZoneEnabled;
    private bool rotationDeadZoneEnabled;

    void Start()
    {
        cameraComponent = gameObject.GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        Debug.Log(AngularDistance());
        GetTargetPosition();
        SetCameraDestination();
        MoveAndRotateCamera();
        //ChangeCameraZoom();
    }
    void GetTargetPosition()
    {
        targetXPos = target.transform.position.x;
        targetYPos = target.transform.position.y;
    }
    void SetCameraDestination()
    {
        cameraPos = new Vector3(targetXPos, targetYPos, -10);
    }
    void MoveAndRotateCamera()
    {
        PositionDeadZoneCheck();
        if (!PositionDeadZoneCheck())
        {
            transform.position = Vector3.Lerp(transform.position, cameraPos, positionLerpAlfa);
        }
        if (!RotationDeadZoneCheck())
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, rotationLerpAlfa);
        }
    }
    void ChangeCameraZoom()
    {
        cameraComponent.orthographicSize = defaultZoom + PlayerControl.instance.GetShipSpeed();
    }
    bool PositionDeadZoneCheck()
    {
        if (DistanceToTarget() < positionDeadZoneMin)
        {
            positionDeadZoneEnabled = true;
        }else if (DistanceToTarget() > positionDeadZoneMax)
        {
            positionDeadZoneEnabled = false;
        }

        return positionDeadZoneEnabled;
    }
    bool RotationDeadZoneCheck()
    {
        if(AngularDistance() > rotationDeadZoneMax)
        {
            rotationDeadZoneEnabled = false;
        }else if(AngularDistance() < rotationDeadZoneMin)
        {
            rotationDeadZoneEnabled = true;
        }

        return rotationDeadZoneEnabled;
    }
    float DistanceToTarget()
    {
        float distanceToTarget;
        var camPos2D =new Vector2(transform.position.x, transform.position.y);
        var targetPos2D = target.transform.position;

        distanceToTarget = Vector2.Distance(targetPos2D, camPos2D);
        return distanceToTarget;
    }
    float AngularDistance()
    {
        float angularDistance;
        float cameraHeading;
        float targetHeading;

        cameraHeading = 360 - transform.eulerAngles.z;
        targetHeading = 360 - target.transform.eulerAngles.z;
        angularDistance = Mathf.Abs(cameraHeading - targetHeading);
        if(angularDistance > 180)
        {
            angularDistance = 360 - angularDistance;
        }
        return angularDistance;
    }
}
