using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;

    private Rigidbody2D playerRB;
    [HideInInspector] public float shipHeading;
    [SerializeField] [Range(0, 10)] int engineControlSensitivity;
    [SerializeField] float engineForce;
    [SerializeField] float turnTorque;
    [SerializeField] float engineTurnTorque;
    [Tooltip("Change this before running the game.")]
    [SerializeField] Vector2 centerOfMass;

    [HideInInspector] public bool playerDocked = false;
    //[HideInInspector] public bool playerInDockZone;
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
        if (!playerDocked)
        {
            ControlEngineSliders();
            MoveShip();
        }
        GetShipHeading();
    }
    void ControlEngineSliders()
    {

        if (Input.GetKey(KeyCode.W))
        {
            if(UIManager.instance.leftEngineSlider.value != UIManager.instance.rightEngineSlider.value)
            {
                UIManager.instance.leftEngineSlider.value = (UIManager.instance.leftEngineSlider.value + UIManager.instance.rightEngineSlider.value) / 2;
                UIManager.instance.rightEngineSlider.value = UIManager.instance.leftEngineSlider.value;
            }
            UIManager.instance.leftEngineSlider.value += (engineControlSensitivity / 5) * Time.deltaTime;
            UIManager.instance.rightEngineSlider.value += (engineControlSensitivity / 5) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (UIManager.instance.leftEngineSlider.value != UIManager.instance.rightEngineSlider.value)
            {
                UIManager.instance.leftEngineSlider.value = (UIManager.instance.leftEngineSlider.value + UIManager.instance.rightEngineSlider.value) / 2;
                UIManager.instance.rightEngineSlider.value = UIManager.instance.leftEngineSlider.value;
            }
            UIManager.instance.leftEngineSlider.value -= (engineControlSensitivity / 5) * Time.deltaTime;
            UIManager.instance.rightEngineSlider.value -= (engineControlSensitivity / 5) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.T))
        {
            UIManager.instance.leftEngineSlider.value += (engineControlSensitivity / 5) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Y))
        {
            UIManager.instance.rightEngineSlider.value += (engineControlSensitivity / 5) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.G))
        {
            UIManager.instance.leftEngineSlider.value -= (engineControlSensitivity / 5) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.H))
        {
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
        //LEFT ENGINE
        playerRB.AddRelativeForce(Vector2.up * engineForce / 2 * leftEngineSliderValue * Time.deltaTime, ForceMode2D.Impulse);
        playerRB.AddTorque(-engineTurnTorque * leftEngineSliderValue * Time.deltaTime, ForceMode2D.Impulse);
        //RIGHT ENGINE
        playerRB.AddRelativeForce(Vector2.up * engineForce / 2 * rightEngineSliderValue * Time.deltaTime, ForceMode2D.Impulse);
        playerRB.AddTorque(engineTurnTorque * rightEngineSliderValue * Time.deltaTime, ForceMode2D.Impulse);
        //RUDDER
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
    public void UndockShip() //Dock ship method is executed by dockzone scripts
    {
        playerDocked = false;
        UIManager.instance.HUD.SetActive(true);
    }
}
