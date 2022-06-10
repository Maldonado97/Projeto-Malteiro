using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("HUD")]
    public GameObject HUD;
    [SerializeField] Image healthBar;
    [Tooltip("Place in descending order")]
    [SerializeField] List<Sprite> healthBarSprites;
    [SerializeField] Image fuelBar;
    [Tooltip("Place in descending order")]
    [SerializeField] List<Sprite> fuelBarSprites;
    [SerializeField] GameObject headingIndicator;
    [SerializeField] GameObject speedIndicator;
    [SerializeField] GameObject dockPrompt;
    private TextMeshProUGUI headingIndicatorText;
    private TextMeshProUGUI speedIndicatorText;
    public GameObject leftEngineSliderUI; //Sliders Controled by the Ship Control Script!!!
    public GameObject rightEngineSliderUI;
    [HideInInspector] public Slider leftEngineSlider;
    [HideInInspector] public Slider rightEngineSlider;
    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;
    [Header("Player Menu")]
    public GameObject playerMenu;
    [Header("Death Screen")]
    [SerializeField] GameObject deathScreen;
    private Image deathScreenImage;
    [SerializeField] Image deathMessage;
    [SerializeField] float blackScreenTransitionSpeed;
    [SerializeField] float deathMessageTransitionSpeed;

    public bool deathScreenActive = false;
    private bool pauseMenuOpen = false;
    private bool playerMenuOpen = false;
    [HideInInspector] public bool playerDocked = false;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        headingIndicatorText = headingIndicator.GetComponent<TextMeshProUGUI>();
        speedIndicatorText = speedIndicator.GetComponent<TextMeshProUGUI>();
        leftEngineSlider = leftEngineSliderUI.GetComponent<Slider>();
        rightEngineSlider = rightEngineSliderUI.GetComponent<Slider>();
        deathScreenImage = deathScreen.GetComponent<Image>();

        SubscribeToEvents();
        dockPrompt.SetActive(false);
        HUD.SetActive(true);
        UpdateHealthBar();
    }
    private void SubscribeToEvents()
    {
        PlayerControl.instance.onDamageTaken += UpdateHealthBar;
        PlayerControl.instance.onPlayerDeath += OnPlayerDeath;
        PlayerControl.instance.onPlayerRespawn += OnPlayerRespawn;
        PlayerControl.instance.onPlayerDocked += OnPlayerDocked;
        PlayerControl.instance.onPlayerUndocked += OnPlayerUndocked;
    }
    void Update()
    {
        UpdateHUD();
        if (Input.GetKeyDown(KeyCode.Escape) && !playerDocked && !playerMenuOpen && !deathScreenActive)
        {
            TogglePauseMenu();
        }
        if (Input.GetKeyDown(KeyCode.Tab) && !playerDocked && !pauseMenuOpen && !deathScreenActive)
        {
            TogglePlayerMenu();
        }
        if (deathScreenActive)
        {
            ActivateDeathScreen();
        }
        else
        {
            DeactivateDeathScreen();
        }
    }
    public void UpdateHUD()
    {
        headingIndicatorText.text = "Heading: " + Mathf.RoundToInt(PlayerControl.instance.shipHeading);
        speedIndicatorText.text = $"Speed: {Mathf.RoundToInt(PlayerControl.instance.GetShipSpeed() * 100) / 10}";
    }
    public void UpdateHealthBar()
    {
        var playerControl = PlayerControl.instance;
        int healthPercentage = Mathf.RoundToInt(((playerControl.health / playerControl.maxHealth) * 100));
        if (healthPercentage <= 0)
        {
            healthBar.sprite = healthBarSprites[10];
        } else
        {
            int i = Mathf.RoundToInt(healthPercentage / 10);
            healthBar.sprite = healthBarSprites[10 - i];
            //Debug.Log($"Player health is: {PlayerControl.instance.Health}");
        }
    }
    public void UpdateFuelBar(float currentFuel, float maxFuel)
    {
        float fuelPercentage = currentFuel / maxFuel;
        int i = Mathf.RoundToInt(10 * fuelPercentage);

        if (currentFuel <= 0)
        {
            fuelBar.sprite = fuelBarSprites[10];
        }
        else
        {
            fuelBar.sprite = fuelBarSprites[10 - i];
            //Debug.Log($"Fuel is at: {fuelPercentage}%");
        }

    }
    public void TogglePauseMenu()
    {
        if (!pauseMenuOpen)
        {
            HUD.SetActive(false);
            pauseMenu.SetActive(true);
            pauseMenuOpen = true;
        }
        else
        {
            HUD.SetActive(true);
            pauseMenu.SetActive(false);
            pauseMenuOpen = false;
        }
    }
    public void TogglePlayerMenu()
    {
        if (!playerMenuOpen)
        {
            HUD.SetActive(false);
            playerMenu.SetActive(true);
            playerMenuOpen = true;
        }
        else
        {
            HUD.SetActive(true);
            PlayerInventoryScreenManager.instance.DeselectAllItems();
            playerMenu.SetActive(false);
            playerMenuOpen = false;
        }
    }
    public void ActivateDockPrompt()
    {
        dockPrompt.SetActive(true);
    }
    public void DeactivateDockPrompt()
    {
        dockPrompt.SetActive(false);
    }
    public void ActivateDeathScreen()
    {
        float deathMessageAlfa;
        float deathScreenAlfa;
        deathScreen.SetActive(true);
        if (deathMessage.color.a < 1)
        {
            deathMessageAlfa = deathMessage.color.a + deathMessageTransitionSpeed * Time.deltaTime;
            deathMessage.color = new Color(deathMessage.color.r, deathMessage.color.g, deathMessage.color.b, deathMessageAlfa);
        }
        if (deathMessage.color.a >= .3f && deathScreenImage.color.a < 1)
        {
            deathScreenAlfa = deathScreenImage.color.a + blackScreenTransitionSpeed * Time.deltaTime;
            deathScreenImage.color = new Color(deathScreenImage.color.r, deathScreenImage.color.g, deathScreenImage.color.b, deathScreenAlfa);
        }
    }
    public void DeactivateDeathScreen()
    {
        deathScreenActive = false;
        deathScreen.SetActive(false);
        deathMessage.color = new Color(deathMessage.color.r, deathMessage.color.g, deathMessage.color.b, 0);
        deathScreenImage.color = new Color(deathScreenImage.color.r, deathScreenImage.color.g, deathScreenImage.color.b, 0);
    }
    //EVENT METHODS
    public void OnPlayerDeath()
    {
        deathScreenActive = true;
        HUD.SetActive(false);
    }
    public void OnPlayerRespawn()
    {
        leftEngineSlider.value = 0;
        rightEngineSlider.value = 0;
        UpdateHealthBar();
        DeactivateDeathScreen();
        HUD.SetActive(true);
    }
    private void OnPlayerDocked()
    {
        HUD.SetActive(false);
        playerDocked = true;
    }
    private void OnPlayerUndocked()
    {
        HUD.SetActive(true);
        playerDocked = false;
    }
}
