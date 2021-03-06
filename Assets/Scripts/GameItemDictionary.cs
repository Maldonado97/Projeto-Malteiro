using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemDictionary : MonoBehaviour
{
    public static GameItemDictionary instance;

    [HideInInspector] public List<string> gameItemNames = new List<string>();
    [HideInInspector] public List<string> gameItemDescriptions = new List<string>();
    [HideInInspector] public List<float> gameItemValues = new List<float>();
    [HideInInspector] public List<float> gameItemWeights = new List<float>();

    [HideInInspector] public List<int> gameItemsByValue = new List<int>();
    private void Start()
    {
        //ID:0 -> Box of Steaks //ID:1 -> Cake //ID:2 -> Contraband //ID:3 -> Empty Crate // ID:4 -> Medicine Box
        //ID:5 -> Scrap Metal
        instance = this;
        //ID:0, Box of Steaks, value 250, weight: 100
        gameItemNames.Add("Box of Steaks");
        gameItemDescriptions.Add("A box of succulent pieces of meat. Good for everyone that consumes these " +
            "delicacies, bad for the animals that became them. Tira o olho Monteiro, voc? n?o pode O_o.");
        gameItemValues.Add(250f);
        gameItemWeights.Add(100f);
        //ID:1, Cake, value: 10, weight: 1.5
        gameItemNames.Add("Cake");
        gameItemDescriptions.Add("Probably just a lie.");
        gameItemValues.Add(10f);
        gameItemWeights.Add(1.5f);
        //ID:2, Contraband, value: 50, weight: 17
        gameItemNames.Add("Contraband");
        gameItemDescriptions.Add("Worth a lot for some reason, just try not to ask too many questions, " +
            "and definitely don't get caught with it.");
        gameItemValues.Add(50f);
        gameItemWeights.Add(17f);
        //ID:3, Empty Crate, value: 5, weight: 5
        gameItemNames.Add("Empty Crate");
        gameItemDescriptions.Add("Well, what can I say? It's just an empty crate");
        gameItemValues.Add(5f);
        gameItemWeights.Add(5f);
        //ID:4, Medicine Box, value: 60, weight: 16
        gameItemNames.Add("Medicine Box");
        gameItemDescriptions.Add("Vital for curing diseases and conducting life saving medical procedures.");
        gameItemValues.Add(60f);
        gameItemWeights.Add(16f);
        //ID:5, Scrap Metal, value: 25, weight: 25
        gameItemNames.Add("Scrap Metal");
        gameItemDescriptions.Add("Once part of something greater, now just a husk of its former glory. " +
            "It's almost like a foreshadowing of what destiny has in store for you.");
        gameItemValues.Add(25f);
        gameItemWeights.Add(25f);
    }
    public void SortByValue()
    {
        int buffer = 0;
        bool sortComplete = false;

        while (!sortComplete)
        {
            buffer = 0;
            for (int i = 0; i < gameItemNames.Count; i++)
            {
                if (gameItemsByValue.Contains(i)) { continue; }
                if (gameItemsByValue.Contains(buffer))
                {
                    buffer = i;
                }
                if (gameItemValues[i] > gameItemValues[buffer])
                {
                    buffer = i;
                }
            }
            gameItemsByValue.Add(buffer);
            if(gameItemsByValue.Count == gameItemValues.Count)
            {
                sortComplete = true;
            }
        }
        Debug.Log("Listing game items by value:");
        foreach(int itemID in gameItemsByValue)
        {
            Debug.Log(gameItemNames[itemID] + " value: " + gameItemValues[itemID]);
        }
    }
}
