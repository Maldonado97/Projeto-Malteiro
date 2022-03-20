using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemDictionary : MonoBehaviour
{
    public static GameItemDictionary instance;

    public List<string> gameItemNames = new List<string>();
    public List<float> gameItemValues = new List<float>();
    public List<float> gameItemWeights = new List<float>();
    private void Start()
    {
        instance = this;
        //ID:0, Cake, value: 10, weight: 1.5
        gameItemNames.Add("Cake");
        gameItemValues.Add(10f);
        gameItemWeights.Add(1.5f);
        //ID:1, Contraband, value: 50, weight: 17
        gameItemNames.Add("Contraband");
        gameItemValues.Add(50f);
        gameItemWeights.Add(17f);
        //ID:2, Empty Crate, value: 5, weight: 5
        gameItemNames.Add("Empty Crate");
        gameItemValues.Add(5f);
        gameItemWeights.Add(5f);
        //ID:3, Medicine Box, value: 60, weight: 16
        gameItemNames.Add("Medicine Box");
        gameItemValues.Add(60f);
        gameItemWeights.Add(16f);
        //ID:4, Scrap Metal, value: 25, weight: 25
        gameItemNames.Add("Scrap Metal");
        gameItemValues.Add(25f);
        gameItemWeights.Add(25f);

    }
}
