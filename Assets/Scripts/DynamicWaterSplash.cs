using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWaterSplash : MonoBehaviour
{
    ParticleSystem waterSplash;
    float waterSplashEmissionRate;
    [Tooltip("What this particle system is attatched to.")]
    [SerializeField] GameObject attatchedShip;
    private Rigidbody2D attatchedShipRB;
    private float attatchedShipHeading;
    // Start is called before the first frame update
    void Start()
    {
        waterSplash = gameObject.GetComponent<ParticleSystem>();
        attatchedShipRB = attatchedShip.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var waterSplashEmisssion = waterSplash.emission;
        Debug.Log(attatchedShipHeading);
        waterSplashEmisssion.rateOverTime = GetShipSpeed() * 10;
    }
    public float GetShipSpeed()
    {
        float forwardSpeed;
        float xSpeed;
        float ySpeed;
        float headingRad;

        xSpeed = attatchedShipRB.velocity.x;
        ySpeed = attatchedShipRB.velocity.y;
        GetShipHeading();
        headingRad = Mathf.Deg2Rad * attatchedShipHeading;
        forwardSpeed = (xSpeed * Mathf.Sin(headingRad)) + (ySpeed * Mathf.Cos(headingRad));

        return forwardSpeed;
    }
    void GetShipHeading()
    {
        attatchedShipHeading = 360 - attatchedShip.transform.eulerAngles.z;
    }
}
