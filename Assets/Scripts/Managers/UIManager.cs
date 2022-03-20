using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] GameObject headingIndicator;
    private TextMeshProUGUI headingIndicatorText;
    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;
    [Header("Player Menu")]
    [SerializeField] GameObject playerMenu;

    private bool pauseMenuOpen;
    private bool playerMenuOpen;
    // Start is called before the first frame update
    void Start()
    {
        headingIndicatorText = headingIndicator.GetComponent<TextMeshProUGUI>();
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
        headingIndicatorText.text = "Heading:" + Mathf.RoundToInt(ShipControl.instance.shipHeading);
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
