using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Tooltip("Determines what the camera will follow (Should be player)")]
    public GameObject target;
    private float targetXPos;
    private float targetYPos;
    void Start()
    {

    }
    void Update()
    {
        targetXPos = target.transform.position.x;
        targetYPos = target.transform.position.y;
        transform.position = new Vector3(targetXPos, targetYPos, -10);
        transform.rotation = target.transform.rotation;
    }
}
