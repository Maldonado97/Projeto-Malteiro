using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopeCityShop1InventoryManager : GeneralDockInventoryManager
{
    public static HopeCityShop1InventoryManager instance;

    public bool canAddItems = false;
    public bool canRemoveItems = true;
    private void Awake()
    {
        instance = this;
        storeCash = 13400;
        StartCoroutine(StartInventoryShuffleCounter(3));
    }
    public void ShuffleInventory()
    {
        int itemsToRemove;
        int itemsToAdd;
        int itemToRemove;
        int itemToAdd;
        int amountToRemove;
        int amountToAdd;


        itemsToRemove = UnityEngine.Random.Range(1, itemIDsInInventory.Count / 2 + 1);
        itemsToAdd = UnityEngine.Random.Range(1, 4);
        if (canShuffleInventory)
        {
            if (canAddItems)
            {
                Debug.Log($"SHUFFLER IS ADDING {itemsToAdd} ITEMS TO INVENTORY");
                for (int i = 0; i < itemsToAdd; i++)
                {
                    itemToAdd = UnityEngine.Random.Range(0, GameItemDictionary.instance.gameItemNames.Count);
                    amountToAdd = UnityEngine.Random.Range(1, 6);
                    Debug.Log($"Shuffler will add {amountToAdd} of " +
                    $"item ID {itemToAdd}: {GameItemDictionary.instance.gameItemNames[itemToAdd]}");
                    AddItemToInventory(itemToAdd, amountToAdd);
                }
            }
            if (itemIDsInInventory.Count > 2 && canRemoveItems)
            {
                Debug.Log($"SHUFFLER IS REMOVING {itemsToRemove} ITEMS FROM INVENTORY");
                for (int i = 0; i < itemsToRemove; i++)
                {
                    itemToRemove = itemIDsInInventory[UnityEngine.Random.Range(0, itemIDsInInventory.Count)];
                    amountToRemove = UnityEngine.Random.Range(1, itemAmount[itemToRemove] + 1);
                    Debug.Log($"Shuffler will remove {amountToRemove} of " +
                    $"item ID {itemToRemove}: {GameItemDictionary.instance.gameItemNames[itemToRemove]}");
                    RemoveItemFromInventory(itemToRemove, amountToRemove);
                }
            }
        }
        StartCoroutine(StartInventoryShuffleCounter(1));
    }
    public IEnumerator StartInventoryShuffleCounter(int timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        ShuffleInventory();
    }
}
