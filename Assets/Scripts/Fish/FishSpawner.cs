using UnityEngine;
using System;

public class FishSpawner : MonoBehaviour
{
    void Awake()
    {
        for(int i = 0; i<fishTypes.Length; i++)
        {
            int fishNumber = 0;
            while(fishNumber < fishTypes[i].fishCount)
            {
                Fish fish = UnityEngine.Object.Instantiate<Fish>(fishPrefab);
                fish.Type = fishTypes[i];
                fish.ResetFish();
                fishNumber++;
            }
        }
    }

    [SerializeField]
    private Fish fishPrefab;

    [SerializeField]
    private Fish.FishType[] fishTypes;
}
