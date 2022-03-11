using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Tooltip("Determines what the camera will follow (Should be player)")]
    public GameObject target;
    public float positionLerpAlfa = .01f;
    public float rotationLerpAlfa = .009f;
    private Vector3 cameraPos;
    private float targetXPos;
    private float targetYPos;

    void LateUpdate()
    {
        GetTargetPosition();
        SetCameraPosition();
        MoveAndRotateCamera();
    }
    void GetTargetPosition()
    {
        targetXPos = target.transform.position.x;
        targetYPos = target.transform.position.y;
    }
    void SetCameraPosition()
    {
        cameraPos = new Vector3(targetXPos, targetYPos, -10);
    }
    void MoveAndRotateCamera()
    {
        transform.position = Vector3.Lerp(transform.position, cameraPos, positionLerpAlfa);
        //transform.rotation = target.transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, rotationLerpAlfa);
    }
}
