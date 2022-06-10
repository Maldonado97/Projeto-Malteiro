using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Tooltip("Determines what the camera will follow (Should be player).")]
    [SerializeField] GameObject target;

    [Header("Camera Dynamic Zoom Settings")]
    [SerializeField] float defaultZoom;
    [Tooltip("Determines how much the camera will zoom out with the target's speed.")]
    [SerializeField] float zoomDistortionIntensity;
    [Tooltip("Determines how aggressively the camera will zoom out with the target's speed.")]
    [SerializeField] float zoomDistortionSensitivity;
    [Tooltip("Determines the minimum target speed that will trigger the camera zoom distortion.")]
    [SerializeField] float zoomDistortionTriggerSpeed;

    [Header("Camera Dynamic Offset Settings")]
    [Tooltip("Set to zoomDistortionTriggerSpeed for best results.")]
    [SerializeField] float cameraOffsetTriggerSpeed;
    [Tooltip("How much the camera moves in front of the player with speed.")]
    [SerializeField] float cameraOffsetIntensity;

    [Header("Camera Movement Deadzone settings")]
    [Tooltip("Determines max distance between the target and the camera before the camera begins" +
        " moving.")]
    [SerializeField] float positionDeadZoneMax; //Ideal = 2
    [Tooltip("Determines at which distance from the target the camera should stop moving.")]
    [SerializeField] float positionDeadZoneMin; //Ideal = 0.2
    [SerializeField] float rotationDeadZoneMax; //Ideal = 20
    [SerializeField] float rotationDeadZoneMin; //Ideal = 1

    [Header("Lerp settings")]
    [Tooltip("Determines how fast the camera will follow the player.")]
    [SerializeField] float positionLerpAlfa; //Ideal = 0.015f
    [Tooltip("Determines how fast the camera will ajust to the player's rotation.")]
    [SerializeField] float rotationLerpAlfa; //Ideal = 0.009f

    private Vector3 cameraPos;
    private Vector3 cameraPosOffset;
    private Camera cameraComponent;
    //private float targetXPos;
    //private float targetYPos;
    private Vector3 targetPos;
    private float cameraHeading;
    private float targetHeading;
    private bool positionDeadZoneEnabled;
    private bool rotationDeadZoneEnabled;
    private bool cameraDistortionDeadzoneEnabled;

    void Start()
    {
        cameraComponent = gameObject.GetComponent<Camera>();
    }
    private void Update()
    {
        GetCameraHeading();
        GetTargetHeading();
        GetTargetPosition();
        SetCameraDestination();
    }
    void FixedUpdate()
    {
        MoveAndRotateCamera();
        DistortCameraZoom();
    }
    void GetTargetPosition()
    {
        float targetXPos = target.transform.position.x;
        float targetYPos = target.transform.position.y;
        targetPos = new Vector3(targetXPos, targetYPos, -10);
    }
    void SetCameraDestination()
    {
        if (GetTargetSpeed() >= cameraOffsetTriggerSpeed)
        {
            float rawOffset = cameraOffsetIntensity * (GetTargetSpeed() - cameraOffsetTriggerSpeed);
            float offsetX = rawOffset * Mathf.Sin(Mathf.Deg2Rad * targetHeading);
            float offsetY = rawOffset * Mathf.Cos(Mathf.Deg2Rad * targetHeading);
            cameraPosOffset = new Vector3(offsetX, offsetY, -10);
        }
        else
        {
            cameraPosOffset = Vector3.zero;
        }
        cameraPos = targetPos + cameraPosOffset;
    }
    void MoveAndRotateCamera()
    {
        PositionDeadZoneCheck();
        if (!PositionDeadZoneCheck() || GetTargetSpeed() >= cameraOffsetTriggerSpeed)
        {
            transform.position = Vector3.Lerp(transform.position, cameraPos, positionLerpAlfa * Time.timeScale);
        }
        if (!RotationDeadZoneCheck())
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, rotationLerpAlfa * Time.timeScale);
        }
    }
    void DistortCameraZoom()
    {
        var distortedZoom = defaultZoom + GetTargetSpeed() * zoomDistortionIntensity;

        if(GetTargetSpeed() > zoomDistortionTriggerSpeed)
        {
            cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, distortedZoom, zoomDistortionSensitivity);
        }
        else
        {
            cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, defaultZoom, zoomDistortionSensitivity);
        }
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

        angularDistance = Mathf.Abs(cameraHeading - targetHeading);
        if(angularDistance > 180)
        {
            angularDistance = 360 - angularDistance;
        }
        return angularDistance;
    }
    public void GetTargetHeading()
    {
        targetHeading = 360 - target.transform.eulerAngles.z;
    }
    public void GetCameraHeading()
    {
        cameraHeading = 360 - transform.eulerAngles.z;
    }
    public float GetTargetSpeed()
    {
        float forwardSpeed;
        float xSpeed;
        float ySpeed;
        float headingRad;
        var targetRB = target.GetComponent<Rigidbody2D>();

        xSpeed = targetRB.velocity.x;
        ySpeed = targetRB.velocity.y;
        headingRad = Mathf.Deg2Rad * targetHeading;
        forwardSpeed = (xSpeed * Mathf.Sin(headingRad)) + (ySpeed * Mathf.Cos(headingRad));


        return forwardSpeed;
    }
}
