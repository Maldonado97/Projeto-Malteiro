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
    private bool pauseMenuOpen;
    // Start is called before the first frame update
    void Start()
    {
        headingIndicatorText = headingIndicator.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenuOpen)
            {
                OpenPauseMenu();
            }
            else
            {
                ClosePauseMenu();
            }
        }
    }
    public void UpdateHUD()
    {
        headingIndicatorText.text = "Heading:" + Mathf.RoundToInt(ShipControl.instance.shipHeading);
    }
    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        pauseMenuOpen = true;
    }
    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        pauseMenuOpen = false;
    }
}
