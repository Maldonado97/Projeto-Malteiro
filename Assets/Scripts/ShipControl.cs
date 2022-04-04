using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    public static ShipControl instance;

    private Rigidbody2D playerRB;
    [HideInInspector] public float shipHeading;
    [SerializeField] [Range(0, 10)] int engineControlSensitivity;
    [SerializeField] float engineForce;
    [SerializeField] float turnTorque;
    [Tooltip("Change this before running the game.")]
    [SerializeField] Vector2 centerOfMass;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        playerRB = gameObject.GetComponent<Rigidbody2D>();
        playerRB.centerOfMass = centerOfMass;
    }

    // Update is called once per frame
    void Update()
    {
        ControlEngineSliders();
        MoveShip();
        GetShipHeading();
    }
    void ControlEngineSliders()
    {
        if (Input.GetKey(KeyCode.W))
        {
            UIManager.instance.leftEngineSlider.value += (engineControlSensitivity / 5) * Time.deltaTime;
            UIManager.instance.rightEngineSlider.value += (engineControlSensitivity / 5) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            UIManager.instance.leftEngineSlider.value -= (engineControlSensitivity / 5) * Time.deltaTime;
            UIManager.instance.rightEngineSlider.value -= (engineControlSensitivity / 5) * Time.deltaTime;
        }
    }
    void MoveShip()
    {
        var leftEngineSliderValue = UIManager.instance.leftEngineSlider.value;
        var rightEngineSliderValue = UIManager.instance.rightEngineSlider.value;

        if (Input.GetKey(KeyCode.W))
        {
            //playerRB.AddRelativeForce(Vector2.up * engineForce * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.S))
        {
            //playerRB.AddRelativeForce(-Vector2.up * (engineForce / 2) * Time.deltaTime, ForceMode2D.Impulse);
        }
        playerRB.AddRelativeForce(Vector2.up * engineForce * leftEngineSliderValue * Time.deltaTime, ForceMode2D.Impulse);
        if (Input.GetKey(KeyCode.D))
        {
            playerRB.AddTorque(-turnTorque * GetShipSpeed() * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerRB.AddTorque(turnTorque * GetShipSpeed() * Time.deltaTime, ForceMode2D.Impulse);
        }
    }
    void GetShipHeading()
    {
        shipHeading = 360 - transform.eulerAngles.z;
    }
    public float GetShipSpeed()
    {
        float forwardSpeed;
        float xSpeed;
        float ySpeed;
        float headingRad;

        xSpeed = playerRB.velocity.x;
        ySpeed = playerRB.velocity.y;
        headingRad = Mathf.Deg2Rad * shipHeading;
        forwardSpeed = (xSpeed * Mathf.Sin(headingRad)) + (ySpeed * Mathf.Cos(headingRad));


        return forwardSpeed;
    }
}
