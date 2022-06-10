using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuManager : MonoBehaviour
{
    [SerializeField] List<BoxButton> menuTabs;
    [Tooltip("Place in the same order as the corresponding menu tabs.")]
    [SerializeField] List<GameObject> menuScreens;
    private void Start()
    {
        UIManager.instance.playerMenu.SetActive(true);
        SelectMenuTab(menuTabs[0]);
        UIManager.instance.playerMenu.SetActive(false);
    }
    private void DeselectAllMenuTabs()
    {
        foreach (BoxButton menuTab in menuTabs)
        {
            menuTab.DeselectButton();
        }
    }
    private void CloseAllMenuScreens()
    {
        foreach(GameObject menuScreen in menuScreens)
        {
            menuScreen.SetActive(false);
        }
    }
    public void OpenMenuScreen(int screenIndex)
    {
        menuScreens[screenIndex].SetActive(true);
    }
    public void SelectMenuTab(BoxButton menuTab)
    {
        int menuTabIndex = menuTabs.Count;
        DeselectAllMenuTabs();
        CloseAllMenuScreens();
        menuTab.SelectButton();

        if (menuTabs.Contains(menuTab))
        {
            menuTabIndex = menuTabs.IndexOf(menuTab); //Could go wrong.
            Debug.Log($"Selected menu tab index is: {menuTabIndex}.");
        }
        else
        {
            Debug.LogError("Player Menu Manager script tried to select a menu tab that wasn't included in " +
                "the menuTabs list.");
        }

        if(menuTabIndex < menuScreens.Count)
        {
            OpenMenuScreen(menuTabIndex);
        }
        else
        {
            Debug.LogError("Player Menu Manager script tried to open a menu screen that wasn't included in " +
                "the menuScreens list.");
        }
    }
}
