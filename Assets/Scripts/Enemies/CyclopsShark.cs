using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsShark : MonoBehaviour
{
    [SerializeField] GameObject sharkZone;

    private Rigidbody2D sharkRB;
    private Collider2D sharkZoneCollider;
    private float sharkSpeed = 1.5f;
    private float turnTorque = 18;
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

        StartCoroutine(ChangeTargetHeading(timeBetweenTurns));
    }
    private void Update()
    {
        GetSharkHeading();
        //Debug.Log($"{GetSharkZoneBearing()}");
        MoveInArea();
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
    float GetSharkZoneBearing()
    {
        float areaRawBearing;
        float sharkZoneBearing;
        Vector2 sharkZone2DPosition;
        Vector2 shark2DPosition;
        Vector2 areaVector;

        sharkZone2DPosition = new Vector2(sharkZone.transform.position.x, sharkZone.transform.position.y);
        shark2DPosition = new Vector2(transform.position.x, transform.position.y);
        areaVector = shark2DPosition - sharkZone2DPosition;
        areaRawBearing = Mathf.Atan2(areaVector.y, areaVector.x) * Mathf.Rad2Deg;
        if(areaRawBearing < 0)
        {
            areaRawBearing += 360;
        }
        sharkZoneBearing = 360 - areaRawBearing - 90;
        if(sharkZoneBearing < 0)
        {
            sharkZoneBearing += 360;
        }

        return sharkZoneBearing;
    }
    public IEnumerator ChangeTargetHeading(int timeBetweenTurns)
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
                if(GetSharkZoneBearing() >= sharkHeading || GetSharkZoneBearing() <= inverseSharkHeading)
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
                if (GetSharkZoneBearing() >= inverseSharkHeading || GetSharkZoneBearing() <= sharkHeading)
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
            targetHeading = GetSharkZoneBearing();
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
            Debug.Log($"Shark is in zone. Setting shark target Heading to {targetHeading}, current " +
                $"shark heading is {sharkHeading}. Shark zone is at {GetSharkZoneBearing()}");
        }else
        {
            Debug.Log($"Shark is outside of zone. Setting shark target Heading to {targetHeading}, current " +
                $"shark heading is {sharkHeading}. Shark zone is at {GetSharkZoneBearing()}");
        }

        yield return new WaitForSeconds(timeBetweenTurns);
        StartCoroutine(ChangeTargetHeading(timeBetweenTurns));
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
