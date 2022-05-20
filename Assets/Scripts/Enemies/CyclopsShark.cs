using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CyclopsShark : MonoBehaviour
{
    [SerializeField] GameObject sharkZone;
    [Header("Hunting")]
    [SerializeField] GameObject target;
    [Tooltip("In degrees.")]
    [SerializeField] int sharkFieldOfView;
    [SerializeField] float sightRange;

    private Rigidbody2D sharkRB;
    private Collider2D sharkZoneCollider;
    private float sharkSpeed = 0; //1.5
    private float turnTorque = 0; //18
    private int timeBetweenTurns = 5;
    private float sharkHeading;
    private float inverseSharkHeading;
    private float inverseTargetHeading;
    private float targetHeading;
    private bool inSharkZone = false;
    private int outOfBoundsCounter = 0;

    public void Start()
    {
        sharkRB = gameObject.GetComponent<Rigidbody2D>();
        sharkZoneCollider = sharkZone.GetComponent<Collider2D>();

        StartCoroutine(SelectRandomTargetHeading(timeBetweenTurns));
    }
    private void Update()
    {
        GetSharkHeading();
        //Debug.Log($"{GetSharkZoneBearing()}");
        MoveInArea();
        LookForTarget();
    }
    public void MoveInArea()
    {
        bool shouldTurnLeft = false;
        bool shouldTurnRight = false;

        MoveForwards();
        if (inverseTargetHeading >= 180)
        {
            if(sharkHeading <= inverseTargetHeading && sharkHeading > targetHeading)
            {
                shouldTurnLeft = true;
                shouldTurnRight = false;
            }
            else
            {
                shouldTurnRight = true;
                shouldTurnLeft = false;
            }
        }
        if(inverseTargetHeading < 180)
        {
            if(sharkHeading >= inverseTargetHeading && sharkHeading < targetHeading)
            {
                shouldTurnRight = true;
                shouldTurnLeft = false;
            }
            else
            {
                shouldTurnLeft = true;
                shouldTurnRight = false;
            }
        }
        if (shouldTurnLeft)
        {
            TurnLeft();
        }
        if (shouldTurnRight)
        {
            TurnRight();
        }
    }
    public void MoveForwards()
    {
        sharkRB.AddRelativeForce(Vector2.up * sharkSpeed * Time.deltaTime, ForceMode2D.Impulse);
    }
    public void TurnLeft()
    {
        sharkRB.AddTorque(turnTorque * Time.deltaTime);
    }
    public void TurnRight()
    {
        sharkRB.AddTorque(-turnTorque * Time.deltaTime);
    }
    public void LookForTarget()
    {
        float fieldOfViewBearing1 = sharkHeading - (sharkFieldOfView / 2);
        float fieldOfViewBearing2 = sharkHeading + (sharkFieldOfView / 2);
        if(fieldOfViewBearing1 < 0)
        {
            fieldOfViewBearing1 += 360;
        }
        if (fieldOfViewBearing2 >= 360)
        {
            fieldOfViewBearing2 -= 360;
        }
        if(fieldOfViewBearing1 > fieldOfViewBearing2)
        {
            if(GetTargetBearing(target) > fieldOfViewBearing1 || GetTargetBearing(target) < fieldOfViewBearing2)
            {
                Debug.Log("TARGET IN SIGHT!");
            }
            else
            {
                Debug.Log("NO TARGET!");
            }
        }
        else if (GetTargetBearing(target) > fieldOfViewBearing1 && GetTargetBearing(target) < fieldOfViewBearing2)
        {
            Debug.Log("TARGET IN SIGHT!");
        }
        else
        {
            Debug.Log("NO TARGET!");
        }
    }
    private void OnDrawGizmos()
    {
        GetSharkHeading();
        Gizmos.color = new Color(1, 0, 0, .15f);
        Gizmos.DrawSphere(transform.position, 3);

        //LINE 1
        float radius = sightRange;
        float line1X = radius * Mathf.Sin(Mathf.Deg2Rad * (sharkHeading + (sharkFieldOfView/2)));
        float line1y = radius * Mathf.Cos(Mathf.Deg2Rad * (sharkHeading + (sharkFieldOfView / 2)));
        Vector3 line1EndPoint = new Vector3(line1X, line1y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + line1EndPoint);

        //LINE 2
        float line2X = radius * Mathf.Sin(Mathf.Deg2Rad * (sharkHeading - (sharkFieldOfView / 2)));
        float line2y = radius * Mathf.Cos(Mathf.Deg2Rad * (sharkHeading - (sharkFieldOfView / 2)));
        Vector3 line2EndPoint = new Vector3(line2X, line2y);
        Gizmos.DrawLine(transform.position, transform.position + line2EndPoint);

        //ARC
        List<Vector3> arcPoints = new List<Vector3>();
        for (int i = (-sharkFieldOfView / 2); i < sharkFieldOfView / 2; i += 10)
        {
            float pointX = radius * Mathf.Sin(Mathf.Deg2Rad * (sharkHeading + i));
            float pointY = radius * Mathf.Cos(Mathf.Deg2Rad * (sharkHeading + i));
            arcPoints.Add(new Vector3(pointX, pointY));
        }
        for(int i = 0; i < (arcPoints.Count - 1); i++)
        {
            Gizmos.DrawLine(transform.position + arcPoints[i], transform.position + arcPoints[i + 1]);
        }
        Gizmos.DrawLine(transform.position + arcPoints[arcPoints.Count - 1], transform.position + line1EndPoint);
    }
    void GetSharkHeading()
    {
        sharkHeading = 360 - transform.eulerAngles.z;
        if (sharkHeading >= 180)
        {
            inverseSharkHeading = sharkHeading - 180;
        }
        else
        {
            inverseSharkHeading = sharkHeading + 180;
        }
    }
    float GetTargetBearing(GameObject target)
    {
        float areaRawBearing;
        float targetBearing;
        Vector2 target2DPosition;
        Vector2 shark2DPosition;
        Vector2 targetVector;

        target2DPosition = new Vector2(target.transform.position.x, target.transform.position.y);
        shark2DPosition = new Vector2(transform.position.x, transform.position.y);
        targetVector = shark2DPosition - target2DPosition;
        areaRawBearing = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg;
        if(areaRawBearing < 0)
        {
            areaRawBearing += 360;
        }
        targetBearing = 360 - areaRawBearing - 90;
        if(targetBearing < 0)
        {
            targetBearing += 360;
        }

        return targetBearing;
    }
    public IEnumerator SelectRandomTargetHeading(int timeBetweenTurns)
    {
        if (inSharkZone)
        {
            targetHeading = sharkHeading + Random.Range(-100f, 100f);
            outOfBoundsCounter = 0;
        }
        if (!inSharkZone && outOfBoundsCounter == 0)
        {
            if(sharkHeading <= 180)
            {
                if(GetTargetBearing(sharkZone) >= sharkHeading || GetTargetBearing(sharkZone) <= inverseSharkHeading)
                {
                    targetHeading = sharkHeading + Random.Range(120f, 180f);
                }
                else
                {
                    targetHeading = sharkHeading - Random.Range(120f, 180f);
                }
            }
            else
            {
                if (GetTargetBearing(sharkZone) >= inverseSharkHeading || GetTargetBearing(sharkZone) <= sharkHeading)
                {
                    targetHeading = sharkHeading - Random.Range(120f, 180f);
                }
                else
                {
                    targetHeading = sharkHeading + Random.Range(120f, 180f);
                }
            }
            outOfBoundsCounter += 1;
        }else if(!inSharkZone && outOfBoundsCounter != 0)
        {
            targetHeading = GetTargetBearing(sharkZone);
        }
        if(targetHeading >= 360)
        {
            targetHeading -= 360;
        }
        if(targetHeading < 0)
        {
            targetHeading = 360 + targetHeading;
        }
        if (targetHeading >= 180)
        {
            inverseTargetHeading = targetHeading - 180;
        }else
        {
            inverseTargetHeading = targetHeading + 180;
        }
        if (inSharkZone)
        {
            //Debug.Log($"Shark is in zone. Setting shark target Heading to {targetHeading}, current " +
                //$"shark heading is {sharkHeading}. Shark zone is at {GetSharkZoneBearing()}");
        }else
        {
            //Debug.Log($"Shark is outside of zone. Setting shark target Heading to {targetHeading}, current " +
                //$"shark heading is {sharkHeading}. Shark zone is at {GetSharkZoneBearing()}");
        }

        yield return new WaitForSeconds(timeBetweenTurns);
        StartCoroutine(SelectRandomTargetHeading(timeBetweenTurns));
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other == sharkZoneCollider)
        {
            inSharkZone = true;
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if(other == sharkZoneCollider)
        {
            inSharkZone = false;
        }
    }
}
