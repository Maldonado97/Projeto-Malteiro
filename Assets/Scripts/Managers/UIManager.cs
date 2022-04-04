using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("HUD")]
    [SerializeField] GameObject headingIndicator;
    [SerializeField] GameObject speedIndicator;
    private TextMeshProUGUI headingIndicatorText;
    private TextMeshProUGUI speedIndicatorText;
    public GameObject leftEngineSliderUI; //Sliders Controled by the Ship Control Script!!!
    public GameObject rightEngineSliderUI;
    [HideInInspector] public Slider leftEngineSlider;
    [HideInInspector] public Slider rightEngineSlider;
    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;
    [Header("Player Menu")]
    [SerializeField] GameObject playerMenu;

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

        pauseMenu.SetActive(false);
        playerMenu.SetActive(false);
    }

    // Update is called once per frame
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
    }
    public void UpdateHUD()
    {
        headingIndicatorText.text = "Heading: " + Mathf.RoundToInt(ShipControl.instance.shipHeading);
        speedIndicatorText.text = "Speed: " + (Mathf.Round(ShipControl.instance.GetShipSpeed() * 100)/100);
    }
    public void TogglePauseMenu()
    {
        if (!pauseMenuOpen)
        {
            pauseMenu.SetActive(true);
            pauseMenuOpen = true;
        }
        else
        {
            pauseMenu.SetActive(false);
            pauseMenuOpen = false;
        }
    }
    public void TogglePlayerMenu()
    {
        if (!playerMenuOpen)
        {
            playerMenu.SetActive(true);
            playerMenuOpen = true;
        }
        else
        {
            playerMenu.SetActive(false);
            playerMenuOpen = false;
        }
    }
}
