using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DockUIManager : MonoBehaviour
{
    public static DockUIManager instance;
    public GameObject dockMenu;
    public List<GeneralDockShopScreenManager> dockShopScreenManagers = new List<GeneralDockShopScreenManager>();

    public void Start()
    {
        SetInstance();
        dockMenu.SetActive(false);
        foreach(GeneralDockShopScreenManager dockShopScreenManager in dockShopScreenManagers)
        {
            dockShopScreenManager.onIventoryItemUICreated += FlashStoreScreen;
        }
    }
    protected abstract void SetInstance();
    public void OnPlayerDocked()
    {
        UIManager.instance.HUD.SetActive(false);
        dockMenu.SetActive(true);
    }
    public void FlashStoreScreen(GeneralDockShopScreenManager dockShopScreenManager)
    {
        if (!dockMenu.activeSelf)
        {
            dockMenu.SetActive(true);
            dockShopScreenManager.storeScreen.SetActive(true);
            dockShopScreenManager.storeScreen.SetActive(false);
            dockMenu.SetActive(false);
        }
        if(dockMenu.activeSelf && !dockShopScreenManager.storeScreen.activeSelf)
        {
            dockShopScreenManager.storeScreen.SetActive(true);
            dockShopScreenManager.storeScreen.SetActive(false);
        }
    }
}
