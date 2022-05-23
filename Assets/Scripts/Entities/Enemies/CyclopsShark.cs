using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CyclopsShark : MonoBehaviour
{
    [SerializeField] GameObject sharkZone;
    [SerializeField] GameObject ownInterface;
    [SerializeField] GameObject woodBurstParticle;
    [Tooltip("In degrees.")]
    [SerializeField] int sharkFieldOfView = 80;
    [SerializeField] float sightRange = 10;
    [SerializeField] float hearingRange = 4;
    [SerializeField] float closeRange = 3.5f;

    //MOVEMENT
    private Rigidbody2D sharkRB;
    private float sharkSpeed; //1.5
    private float chaseSpeed = 2.1f; //2.1f
    private float normalSpeed = 1.5f; //1.5
    private float coolDownSpeed = 0.5f;
    private float turnTorque; //18
    private float normalTurnTorque = 18;
    private float closeRangeTurnTorque = 13;
    private float chasingSoundTurnTorque = 20;
    //HEADING
    private float sharkHeading;
    private float inverseSharkHeading;
    private float inverseTargetHeading;
    private float targetHeading;
    //NAVIGATION
    private Collider2D sharkZoneCollider;
    private bool inSharkZone = false;
    private int timeBetweenTurns = 5;
    private int outOfBoundsCounter = 0;
    //ATTACK
    private GameObject target;
    private Rigidbody2D targetRB;
    private Collider2D targetCollider;
    private EntityController targetEntityController;
    private float distanceToTarget;
    private bool targetAquired = false;
    private bool targetIsSound = false;
    private bool chasingTarget = false;
    private bool targetAtCloseRange = false;
    private float damageVariation = .2f; //Percentage of how much base damage can vary
    private float biteDamage = 25;
    private float biteForce = 2;
    private float biteCooldownTime = 3;
    private bool inBiteCooldown = false;
    //ANIMATION
    private Animator animator;
    //OPTIMIZATION
    private GameObject player;
    private bool inStandby;
    [SerializeField] private float activationRadius;
    public void Start()
    {
        sharkRB = gameObject.GetComponent<Rigidbody2D>();
        //animator = gameObject.GetComponent<Animator>();
        animator = ownInterface.GetComponent<Animator>();
        sharkZoneCollider = sharkZone.GetComponent<Collider2D>();
        player = PlayerControl.instance.gameObject;
        timeBetweenTurns = 5;
        StartCoroutine(SelectRandomTargetHeading(timeBetweenTurns));
    }
    private void Update()
    {
        if(GetGameObjectDistance(player) > activationRadius && inSharkZone)
        {
            inStandby = true;
            ownInterface.SetActive(false);
        }
        else
        {
            inStandby = false;
            ownInterface.SetActive(true);
        }
        if (!inStandby)
        {
            GetSharkHeading();
            //Debug.Log($"{GetSharkZoneBearing()}");
            if (inSharkZone)
            {
                GetClosestTarget();
                if (targetAquired)
                {
                    ChaseTarget();
                    chasingTarget = true;
                }
                else
                {
                    chasingTarget = false;
                }
            }
            else
            {
                targetAquired = false;
                chasingTarget = false;
                targetIsSound = false;
            }
            MoveInDesiredHeading();
            UpdateAnimatorParameters();
        }
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
    float GetGameObjectBearing(GameObject target)
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
        if (areaRawBearing < 0)
        {
            areaRawBearing += 360;
        }
        targetBearing = 360 - areaRawBearing - 90;
        if (targetBearing < 0)
        {
            targetBearing += 360;
        }

        return targetBearing;
    }
    float GetGameObjectDistance(GameObject target)
    {
        float distance;

        distance = Vector3.Distance(target.transform.position, transform.position);

        return distance;
    }
    public void MoveForwards()
    {
        if (chasingTarget && !targetIsSound)
        {
            sharkSpeed = chaseSpeed;
        }
        else if (inBiteCooldown)
        {
            sharkSpeed = coolDownSpeed;
        }else
        {
            sharkSpeed = normalSpeed;
        }
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
    public bool CheckIfTargetInView(GameObject possibleTarget)
    {
        bool targetInView;
        //GetClosestTarget();
        float fieldOfViewBearing1 = sharkHeading - (sharkFieldOfView / 2);
        float fieldOfViewBearing2 = sharkHeading + (sharkFieldOfView / 2);
        //float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (fieldOfViewBearing1 < 0)
        {
            fieldOfViewBearing1 += 360;
        }
        if (fieldOfViewBearing2 >= 360)
        {
            fieldOfViewBearing2 -= 360;
        }
        if (fieldOfViewBearing1 > fieldOfViewBearing2)
        {
            if (GetGameObjectBearing(possibleTarget) > fieldOfViewBearing1 || GetGameObjectBearing(possibleTarget) < fieldOfViewBearing2)
            {
                targetInView = true;
                //Debug.Log("TARGET IN SIGHT!");
            }
            else
            {
                targetInView = false;
                //Debug.Log("NO TARGET!");
            }
        }
        else if (GetGameObjectBearing(possibleTarget) > fieldOfViewBearing1 && GetGameObjectBearing(possibleTarget) < fieldOfViewBearing2)
        {
            targetInView = true;
            //Debug.Log("TARGET IN SIGHT!");
        }
        else
        {
            targetInView = false;
            //Debug.Log("NO TARGET!");
        }
        return targetInView;
    }
    public void GetClosestTarget()
    {
        GameObject closestTarget = null;
        List<GameObject> possibleVisualTargets = new List<GameObject>();
        List<GameObject> possibleAuditiveTargets = new List<GameObject>();
        float closestTargetDistance = 0;
        float possibleTargetDistance = 0;
        if(GameObject.FindGameObjectsWithTag("Player").Length != 0)
        {
            foreach (GameObject possibleTarget in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (GetGameObjectDistance(possibleTarget) <= sightRange)
                {
                    if (CheckIfTargetInView(possibleTarget))
                    {
                        possibleVisualTargets.Add(possibleTarget);
                    }else if(GetGameObjectDistance(possibleTarget) <= hearingRange)
                    {
                        possibleAuditiveTargets.Add(possibleTarget);
                    }
                }
            }
        }
        if (GameObject.FindGameObjectsWithTag("Ship").Length != 0)
        {
            foreach (GameObject possibleTarget in GameObject.FindGameObjectsWithTag("Ship"))
            {
                if (GetGameObjectDistance(possibleTarget) <= sightRange)
                {
                    if (CheckIfTargetInView(possibleTarget))
                    {
                        possibleVisualTargets.Add(possibleTarget);
                    }
                    else if (GetGameObjectDistance(possibleTarget) <= hearingRange)
                    {
                        possibleAuditiveTargets.Add(possibleTarget);
                    }
                }
            }
        }
        if (GameObject.FindGameObjectsWithTag("Fish").Length != 0)
        {
            foreach (GameObject possibleTarget in GameObject.FindGameObjectsWithTag("Fish"))
            {
                if (GetGameObjectDistance(possibleTarget) <= sightRange)
                {
                    if (CheckIfTargetInView(possibleTarget))
                    {
                        possibleVisualTargets.Add(possibleTarget);
                    }
                    else if (GetGameObjectDistance(possibleTarget) <= hearingRange)
                    {
                        possibleAuditiveTargets.Add(possibleTarget);
                    }
                }
            }
        }
        if(possibleVisualTargets.Count > 0 || possibleAuditiveTargets.Count > 0)
        {
            targetAquired = true;

            if(possibleVisualTargets.Count > 0) //VISUAL TARGETS HAVE PRIORITY OVER AUDITIVE TARGETS
            {
                targetIsSound = false;
                for (int i = 0; i < possibleVisualTargets.Count; i++)
                {
                    if (i == 0)
                    {
                        closestTarget = possibleVisualTargets[0];
                    }
                    closestTargetDistance = Vector3.Distance(closestTarget.transform.position, transform.position);
                    possibleTargetDistance = Vector3.Distance(possibleVisualTargets[i].transform.position, transform.position);
                    if (closestTargetDistance > possibleTargetDistance)
                    {
                        closestTarget = possibleVisualTargets[i];
                        closestTargetDistance = possibleTargetDistance;
                    }
                }
            }
            else //AUDITIVE TARGETS
            {
                targetIsSound = true;
                for (int i = 0; i < possibleAuditiveTargets.Count; i++)
                {
                    if (i == 0)
                    {
                        closestTarget = possibleAuditiveTargets[0];
                    }
                    closestTargetDistance = Vector3.Distance(closestTarget.transform.position, transform.position);
                    possibleTargetDistance = Vector3.Distance(possibleAuditiveTargets[i].transform.position, transform.position);
                    if (closestTargetDistance > possibleTargetDistance)
                    {
                        closestTarget = possibleAuditiveTargets[i];
                        closestTargetDistance = possibleTargetDistance;
                    }
                }
            }
            target = closestTarget;
            targetRB = target.GetComponent<Rigidbody2D>();
            targetCollider = target.GetComponent<Collider2D>();
            targetEntityController = target.GetComponent<EntityController>();
            distanceToTarget = closestTargetDistance;
            //Debug.Log($"Closest target is: {closestTarget.name}, at a distance of {closestTargetDistance}");
        }
        else
        {
            targetAquired = false;
            targetIsSound = false;
        }
        CheckIfTargetInCloseRange();
    }
    public void CheckIfTargetInCloseRange() //In LookForTarget method
    {
        if(targetAquired && !targetIsSound && distanceToTarget <= closeRange)
        {
            targetAtCloseRange = true;
        }
        else
        {
            targetAtCloseRange = false;
        }
        if (targetAtCloseRange)
        {
            turnTorque = closeRangeTurnTorque;
            //Debug.Log($"Turn Torque is closeRangeTurnTorque");
        }else if (targetIsSound)
        {
            turnTorque = chasingSoundTurnTorque;
            //Debug.Log($"Turn Torque is chasingSoundTurnTorque");
        }
        else
        {
            turnTorque = normalTurnTorque;
            //Debug.Log($"Turn Torque is normalTurnTorque");
        }
    }
    public void ChaseTarget()
    {
        SetDesiredHeading(GetGameObjectBearing(target));
    }
    public void BiteTarget()
    {
        //DAMAGE
        float minimumBiteDamage = biteDamage * (1 - damageVariation);
        float maximumBiteDamage = biteDamage * (1 + damageVariation);
        targetEntityController.DamageEntity(Mathf.RoundToInt(Random.Range(minimumBiteDamage, maximumBiteDamage)));
        //FORCE
        Vector3 forceVector = target.transform.position - transform.position;
        Vector2 forceVector2D = new Vector2(forceVector.x, forceVector.y);
        targetRB.AddForce(forceVector2D * biteForce, ForceMode2D.Impulse);

        //PARTICLE
        float particleOffset = 3;
        float offsetX = particleOffset * Mathf.Sin(Mathf.Deg2Rad * sharkHeading);
        float offsetY = particleOffset * Mathf.Cos(Mathf.Deg2Rad * sharkHeading);
        Vector3 particleSpawnPosition = transform.position + new Vector3(offsetX, offsetY);
        Instantiate(woodBurstParticle, transform.position + forceVector / 2, new Quaternion(0, 0, 0, 0));
        inBiteCooldown = true;
        StartCoroutine(BiteCooldown(biteCooldownTime));
    }
    public void SetDesiredHeading(float rawAngle)
    {
        if (rawAngle >= 360)
        {
            targetHeading = rawAngle - 360;
        }
        else if (rawAngle < 0)
        {
            targetHeading = 360 + rawAngle;
        }
        else
        {
            targetHeading = rawAngle;
        }
        //Debug.Log($"{targetHeading}");
        //INVERSE TARGET HEADING
        if (targetHeading >= 180)
        {
            inverseTargetHeading = targetHeading - 180;
        }
        else
        {
            inverseTargetHeading = targetHeading + 180;
        }
    }
    public void MoveInDesiredHeading()
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
    public void UpdateAnimatorParameters()
    {
        if (targetAtCloseRange)
        {
            animator.SetBool("TargetAtCloseRange", true);
        }
        else
        {
            animator.SetBool("TargetAtCloseRange", false);
        }
        if(inBiteCooldown)
        {
            animator.SetBool("InBiteCooldown", true);
        }
        else
        {
            animator.SetBool("InBiteCooldown", false);
        }
    }
    //IENUMERATORS
    public IEnumerator SelectRandomTargetHeading(int timeBetweenTurns)
    {
        if (!chasingTarget && !inStandby)
        {
            //RANDOM MOVE MODE
            if (inSharkZone)
            {
                SetDesiredHeading(sharkHeading + Random.Range(-100f, 100f));
                //targetHeading = sharkHeading + Random.Range(-100f, 100f);
                outOfBoundsCounter = 0;
            }
            //RETURN TO AREA MODE
            if (!inSharkZone && outOfBoundsCounter == 0)
            {
                if (sharkHeading <= 180)
                {
                    if (GetGameObjectBearing(sharkZone) >= sharkHeading || GetGameObjectBearing(sharkZone) <= inverseSharkHeading)
                    {
                        SetDesiredHeading(sharkHeading + Random.Range(120f, 180f));
                        //targetHeading = sharkHeading + Random.Range(120f, 180f);
                    }
                    else
                    {
                        SetDesiredHeading(sharkHeading - Random.Range(120f, 180f));
                        //targetHeading = sharkHeading - Random.Range(120f, 180f);
                    }
                }
                else
                {
                    if (GetGameObjectBearing(sharkZone) >= inverseSharkHeading || GetGameObjectBearing(sharkZone) <= sharkHeading)
                    {
                        SetDesiredHeading(sharkHeading - Random.Range(120f, 180f));
                        //targetHeading = sharkHeading - Random.Range(120f, 180f);
                    }
                    else
                    {
                        SetDesiredHeading(sharkHeading + Random.Range(120f, 180f));
                        //targetHeading = sharkHeading + Random.Range(120f, 180f);
                    }
                }
                outOfBoundsCounter += 1;
            }
            else if (!inSharkZone && outOfBoundsCounter != 0)
            {
                SetDesiredHeading(GetGameObjectBearing(sharkZone));
                //targetHeading = GetTargetBearing(sharkZone);
            }
        }
        //if (inSharkZone) DON'T REMOVE THIS
        //{
        //Debug.Log($"Shark is in zone. Setting shark target Heading to {targetHeading}, current " +
        //$"shark heading is {sharkHeading}. Shark zone is at {GetSharkZoneBearing()}");
        //}else
        //{
        //Debug.Log($"Shark is outside of zone. Setting shark target Heading to {targetHeading}, current " +
        //$"shark heading is {sharkHeading}. Shark zone is at {GetSharkZoneBearing()}");
        //}

        yield return new WaitForSeconds(timeBetweenTurns);
        StartCoroutine(SelectRandomTargetHeading(timeBetweenTurns));
    }
    public IEnumerator BiteCooldown(float coolDown)
    {
        yield return new WaitForSeconds(coolDown);
        inBiteCooldown = false;
    }
    //EVENT METHODS
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other == sharkZoneCollider)
        {
            inSharkZone = true;
        }
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        var otherCollider = other.collider;
        if(otherCollider == targetCollider && chasingTarget)
        {
            //Debug.Log($"Biting target: {otherCollider.gameObject.name}");
            BiteTarget();
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if(other == sharkZoneCollider)
        {
            inSharkZone = false;
        }
    }
    private void OnDrawGizmos()
    {
        GetSharkHeading();

        //SIGHT RANGE
        DrawCircleSection(sightRange, sharkFieldOfView, sharkHeading, Color.yellow);
        //CLOSE RANGE
        DrawCircleSection(closeRange, sharkFieldOfView, sharkHeading, Color.red);
        //HEARING RANGE
        DrawCircle(hearingRange, Color.green);
        //ACTIVATION RADIUS
        DrawCircle(activationRadius, Color.cyan);
    }
    public void DrawCircleSection(float radius, float arcAperture, float arcCenter, Color color)
    {
        Gizmos.color = color;
        //LINE 1
        float line1X = radius * Mathf.Sin(Mathf.Deg2Rad * (arcCenter - (arcAperture / 2)));
        float line1y = radius * Mathf.Cos(Mathf.Deg2Rad * (arcCenter - (arcAperture / 2)));
        Vector3 line1EndPoint = new Vector3(line1X, line1y);
        Gizmos.DrawLine(transform.position, transform.position + line1EndPoint);
        //LINE 2
        float line2X = radius * Mathf.Sin(Mathf.Deg2Rad * (arcCenter + (arcAperture / 2)));
        float line2y = radius * Mathf.Cos(Mathf.Deg2Rad * (arcCenter + (arcAperture / 2)));
        Vector3 line2EndPoint = new Vector3(line2X, line2y);
        Gizmos.DrawLine(transform.position, transform.position + line2EndPoint);
        //ARC
        List<Vector3> arcPoints = new List<Vector3>();
        for (int i = (-sharkFieldOfView / 2); i < sharkFieldOfView / 2; i += 10)
        {
            float pointX = radius * Mathf.Sin(Mathf.Deg2Rad * (arcCenter + i));
            float pointY = radius * Mathf.Cos(Mathf.Deg2Rad * (arcCenter + i));
            arcPoints.Add(new Vector3(pointX, pointY));
        }
        for (int i = 0; i < (arcPoints.Count - 1); i++)
        {
            Gizmos.DrawLine(transform.position + arcPoints[i], transform.position + arcPoints[i + 1]);
        }
        Gizmos.DrawLine(transform.position + arcPoints[arcPoints.Count - 1], transform.position + line2EndPoint);
    }
    public void DrawCircle(float radius, Color color)
    {
        Gizmos.color = color;
        //ARC
        List<Vector3> arcPoints = new List<Vector3>();
        for (int i = 0; i <= 360; i += 10)
        {
            float pointX = radius * Mathf.Sin(Mathf.Deg2Rad * i);
            float pointY = radius * Mathf.Cos(Mathf.Deg2Rad * i);
            arcPoints.Add(new Vector3(pointX, pointY));
        }
        for (int i = 0; i < (arcPoints.Count - 1); i++)
        {
            Gizmos.DrawLine(transform.position + arcPoints[i], transform.position + arcPoints[i + 1]);
        }
        Gizmos.DrawLine(transform.position + arcPoints[arcPoints.Count - 1], transform.position + arcPoints[0]);
    }
}
