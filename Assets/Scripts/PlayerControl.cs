using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : EntityController
{
    public static PlayerControl instance;

    private Rigidbody2D playerRB;
    [HideInInspector] public Collider2D playerCollider;
    [SerializeField] GameObject ownInterface;
    [SerializeField] List <Collider2D> shipColliders = new List<Collider2D>();
    [SerializeField] List<GameObject> shipInterfaces = new List<GameObject>();
    //MOVEMENT
    [SerializeField] [Range(0, 10)] int engineControlSensitivity;
    [SerializeField] float engineForce = 2;
    [SerializeField] float turnTorque = .15f;
    [SerializeField] float engineTurnTorque = .1f;
    private float reverseDampner = 0.5f;
    [HideInInspector] public float AverageFuelConsumption = 2.65f; //Liters per second in maxThrottle
    [HideInInspector] public float currentFuelConsumption;
    private bool outOfFuel;
    [HideInInspector] public float shipHeading;
    //DOCKING
    [HideInInspector] public DockZone currentDockZone = null;
    [HideInInspector] public bool playerInDockZone = false;
    [HideInInspector] public bool playerDocked = false;
    //DEATH
    public Vector3 respawnPosition = new Vector3(32, -13);
    public float respawnRotation = 200;
    [HideInInspector] public bool playerIsDead = false;
    private int respawnTime = 4;

    public event Action onDamageTaken;
    public event Action onPlayerDeath;
    public event Action onPlayerRespawn;
    void Awake()
    {
        instance = this;
        SelectShip("Survival Ship");
        playerRB = gameObject.GetComponent<Rigidbody2D>();
        maxHealth = 1000;
        health = 500;
        //playerCollider = gameObject.GetComponent<Collider2D>();
        //Debug.Log(Health);
    }
    void LateUpdate()
    {
        if (!playerIsDead)
        {
            if (!playerDocked)
            {
                ControlEngineSliders();
                MoveShip();
            }
            if (playerInDockZone)
            {
                if (currentDockZone.playerInZone && Input.GetKeyDown(KeyCode.V))
                {
                    DockShip();
                }
            }
        }
        GetShipHeading();
    }
    void SelectShip(string shipName)
    {
        foreach (Collider2D shipCollider in shipColliders)
        {
            shipCollider.enabled = false;
        }
        foreach (GameObject shipInterface in shipInterfaces)
        {
            shipInterface.SetActive(false);
        }

        if (shipName == "Wooden Ship") //ID: 0
        {
            shipColliders[0].enabled = true;
            playerCollider = shipColliders[0];
            shipInterfaces[0].SetActive(true);
            ownInterface = shipInterfaces[0];
        }else if (shipName == "Survival Ship") //ID: 1
        {
            shipColliders[1].enabled = true;
            playerCollider = shipColliders[1];
            shipInterfaces[1].SetActive(true);
            ownInterface = shipInterfaces[1];
        }
        else
        {
            Debug.LogError("Non-existant ship selected in playerControl script.");
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

        //LEFT ENGINE
        if (outOfFuel)
        {
            leftEngineSliderValue *= .1f;
        }
        if(leftEngineSliderValue < 0)
        {
            leftEngineSliderValue *= reverseDampner;
        }
        playerRB.AddRelativeForce(Vector2.up * engineForce / 2 * leftEngineSliderValue * Time.deltaTime, ForceMode2D.Impulse);
        playerRB.AddTorque(-engineTurnTorque * leftEngineSliderValue * Time.deltaTime, ForceMode2D.Impulse);
        //RIGHT ENGINE
        if (outOfFuel)
        {
            rightEngineSliderValue *= .1f;
        }
        if (rightEngineSliderValue < 0)
        {
            rightEngineSliderValue *= reverseDampner;
        }
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
        ConsumeFuel(leftEngineSliderValue);
        ConsumeFuel(rightEngineSliderValue);
    }
    void ConsumeFuel(float EngineSliderValue)
    {
        float highSpeedMultiplier = 1.2f;
        float economicMultiplier = .8f;
        float lowSpeedMultiplier = 1.2f;
        //var fuel = PlayerInventoryManager.instance.fuel;

        currentFuelConsumption = (Mathf.Abs(EngineSliderValue) * AverageFuelConsumption) / 2;

        if (Mathf.Abs(EngineSliderValue) < .1)
        {
            currentFuelConsumption = (.1f * AverageFuelConsumption)/2;
        }
        if (Mathf.Abs(EngineSliderValue) < .4)
        {
            currentFuelConsumption *= lowSpeedMultiplier;
        }
        if (Mathf.Abs(EngineSliderValue) < .8)
        {
            currentFuelConsumption *= economicMultiplier;
        }
        if (Mathf.Abs(EngineSliderValue) >= .8)
        {
            currentFuelConsumption *= highSpeedMultiplier;
        }
        if(EngineSliderValue < 0)
        {
            currentFuelConsumption *= reverseDampner;
        }
        if(PlayerInventoryManager.instance.fuel > 0)
        {
            PlayerInventoryManager.instance.fuel -= currentFuelConsumption * Time.deltaTime;
            //Debug.Log($"Fuel: {PlayerInventoryManager.instance.fuel}/{PlayerInventoryManager.instance.maxFuel}");
            UIManager.instance.UpdateFuelBar(PlayerInventoryManager.instance.fuel, PlayerInventoryManager.instance.maxFuel);
            outOfFuel = false;
        }
        else
        {
            outOfFuel = true;
        }
    }
    public void DockShip()
    {
        currentDockZone.DockShip(gameObject);
        currentDockZone.dockUIManager.OnPlayerDocked();
    }
    public void UndockShip() //Dock ship method is executed by dockzone scripts
    {
        playerDocked = false;
        UIManager.instance.HUD.SetActive(true);
    }
    public override void DamageEntity(float damage)
    {
        base.DamageEntity(damage);
        onDamageTaken?.Invoke();
    }
    public override void KillEntity()
    {
        playerIsDead = true;
        Debug.Log("Player Dead");
        gameObject.tag = "Dead Player";
        playerCollider.enabled = false;
        onPlayerDeath?.Invoke();
        StartCoroutine(Respawn());
    }
    //IENUMERATORS
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        
        playerRB.velocity = Vector2.zero;
        playerRB.angularVelocity = 0;
        //UIManager.instance.leftEngineSlider.value = 0;
        //UIManager.instance.rightEngineSlider.value = 0;
        health = maxHealth;
        PlayerInventoryManager.instance.fuel = PlayerInventoryManager.instance.maxFuel;
        //UIManager.instance.UpdateHealthBar();
        playerIsDead = false;
        gameObject.tag = "Player";
        playerCollider.enabled = true;
        transform.position = respawnPosition;
        transform.eulerAngles = new Vector3(0, 0, respawnRotation);
        yield return new WaitForSeconds(2); //Time it takes for the camera to zip past the screen
        onPlayerRespawn?.Invoke();
    }
}
