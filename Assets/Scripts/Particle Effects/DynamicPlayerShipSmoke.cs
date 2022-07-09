using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPlayerShipSmoke : MonoBehaviour
{
    ParticleSystem shipSmoke;
    [SerializeField] float smokeLerpAlfa = .1f;
    private float maxEmission = 150;
    private float baseEmission = 15;
    [Tooltip("What this particle system is attatched to.")]
    [SerializeField] GameObject attatchedShip;
    private Rigidbody2D attatchedShipRB;
    //private float attatchedShipHeading;
    float emissionRate = 0;
    // Start is called before the first frame update
    void Start()
    {
        attatchedShipRB = attatchedShip.GetComponent<Rigidbody2D>();
        shipSmoke = gameObject.GetComponent<ParticleSystem>();
    }
    //1 - 26
    // Update is called once per frame
    void Update()
    {
        float leftEngineSlider = UIManager.instance.leftEngineSlider.value;
        float rightEngineSlider = UIManager.instance.rightEngineSlider.value;
        float engineForce = (Mathf.Abs(leftEngineSlider) / 2) + (Mathf.Abs(rightEngineSlider) / 2);
        //Debug.Log($"EngineForce = {engineForce}");
        float differential = (engineForce * 26) / (Mathf.Abs(GetShipSpeed() * 10));
        //Debug.Log($"Differential = {differential}");
        if (differential >= 2)
        {
            emissionRate = Mathf.Lerp(emissionRate, maxEmission * engineForce * 2, smokeLerpAlfa);
        }
        else
        {
            emissionRate = Mathf.Lerp(emissionRate, maxEmission * engineForce, smokeLerpAlfa);
        }
        
        if(emissionRate < baseEmission)
        {
            emissionRate = Mathf.Lerp(emissionRate, baseEmission, smokeLerpAlfa); ;
        }
        var smokeEmission = shipSmoke.emission;
        smokeEmission.rateOverTime = emissionRate;
    }
    public float GetShipSpeed()
    {
        float forwardSpeed;
        float xSpeed;
        float ySpeed;
        float headingRad;

        xSpeed = attatchedShipRB.velocity.x;
        ySpeed = attatchedShipRB.velocity.y;
        headingRad = Mathf.Deg2Rad * GetShipHeading();
        forwardSpeed = (xSpeed * Mathf.Sin(headingRad)) + (ySpeed * Mathf.Cos(headingRad));

        if (forwardSpeed > 0)
        {
            return Vector2.SqrMagnitude(attatchedShipRB.velocity);
        }
        else
        {
            return -Vector2.SqrMagnitude(attatchedShipRB.velocity);
        }
    }
    public float GetShipHeading()
    {
        float attatchedShipHeading = 360 - attatchedShip.transform.eulerAngles.z;

        return attatchedShipHeading;
    }
}
