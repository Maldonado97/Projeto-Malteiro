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
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        headingIndicatorText = headingIndicator.GetComponent<TextMeshProUGUI>();
        speedIndicatorText = speedIndicator.GetComponent<TextMeshProUGUI>();
        leftEngineSlider = leftEngineSliderUI.GetComponent<Slider>();
        rightEngineSlider = rightEngineSliderUI.GetComponent<Slider>();
        deathScreenImage = deathScreen.GetComponent<Image>();

        SubscribeToEvents();
        dockPrompt.SetActive(false);
        HUD.SetActive(true);
    }
    private void SubscribeToEvents()
    {
        PlayerControl.instance.onDamageTaken += UpdateHealthBar;
        PlayerControl.instance.onPlayerDeath += OnPlayerDeath;
        PlayerControl.instance.onPlayerRespawn += OnPlayerRespawn;
    }
    void Update()
    {
        UpdateHUD();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
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
        speedIndicatorText.text = "Speed: " + (Mathf.Round(PlayerControl.instance.GetShipSpeed() * 100)/100);
    }
    public void UpdateHealthBar()
    {
        if(PlayerControl.instance.Health <= 0)
        {
            healthBar.sprite = healthBarSprites[10];
        }else
        {
            int i = Mathf.RoundToInt(PlayerControl.instance.Health / 10);
            healthBar.sprite = healthBarSprites[10 - i];
            Debug.Log($"Player health is: {PlayerControl.instance.Health}");
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
        if(deathMessage.color.a < 1)
        {
            deathMessageAlfa = deathMessage.color.a + deathMessageTransitionSpeed * Time.deltaTime;
            deathMessage.color = new Color(deathMessage.color.r, deathMessage.color.g, deathMessage.color.b, deathMessageAlfa);
        }
        if(deathMessage.color.a >= .3f && deathScreenImage.color.a < 1)
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
    public void OnPlayerDeath()
    {
        deathScreenActive = true;
    }
    public void OnPlayerRespawn()
    {
        leftEngineSlider.value = 0;
        rightEngineSlider.value = 0;
        UpdateHealthBar();
        DeactivateDeathScreen();
    }
}
