using UnityEngine;
using System;

public class IdleManager : MonoBehaviour
{
    [HideInInspector]
    public int length;
    [HideInInspector]
    public int strength;
    [HideInInspector]
    public int offlineEarnings;
    [HideInInspector]
    public int lengthCost;
    [HideInInspector]
    public int strengthCost;
    [HideInInspector]
    public int offlineEarningsCost;
    [HideInInspector]
    public int wallet;
    [HideInInspector]
    public int totalGain;

    private int[] costs = new int[]
    {
        120, 
        151, 
        197,
        251, 
        350, 
        450, 
        578, 
        700, 
        900,
        1200,
        1600,
        2100
    };

    public static IdleManager instance;

    void Awake()
    {   
        if(IdleManager.instance)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
        else
        {
            IdleManager.instance = this;
        }

        length = -PlayerPrefs.GetInt("Length", 30);
        strength = PlayerPrefs.GetInt("Strength", 3);
        offlineEarnings = PlayerPrefs.GetInt("Offline", 3);
        lengthCost = costs[-length / 10 - 3];
        strengthCost = costs[strength - 3];
        offlineEarningsCost = costs[offlineEarnings - 3];
        wallet = PlayerPrefs.GetInt("Wallet", 0);
    }

    private void OnApplicationPause(bool paused) 
    {
        if(paused)    
        {
            DateTime now = DateTime.Now;
            PlayerPrefs.SetString("Date", now.ToString());
        }
        else
        {
            string @string = PlayerPrefs.GetString("Date", string.Empty);
            if(@string != string.Empty)
            {
                DateTime d = DateTime.Parse(@string);
                totalGain = Convert.ToInt32((DateTime.Now - d).TotalMinutes * offlineEarnings + 1.0);
                ScreensManager.instance.ChangeScreen(ScreensManager.Screens.RETURN);
            }
        }
    }

    private void OnApplicationQuit() 
    {
        OnApplicationPause(true);
    }

    public void BuyLength()
    {
        length -= 10;
        wallet -= lengthCost;
        lengthCost = costs[-length / 10 - 3];
        PlayerPrefs.SetInt("Length", -length);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreensManager.instance.ChangeScreen(ScreensManager.Screens.MAIN);
    }

    public void BuyStrength()
    {
        strength++;
        wallet -= strengthCost;
        strengthCost = costs[strength - 3];
        PlayerPrefs.SetInt("Strength", strength);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreensManager.instance.ChangeScreen(ScreensManager.Screens.MAIN);
    }

    public void BuyOfflineEarnings()
    {
        offlineEarnings++;
        wallet -= offlineEarningsCost;
        offlineEarningsCost = costs[offlineEarnings - 3];
        PlayerPrefs.SetInt("Offline", offlineEarnings);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreensManager.instance.ChangeScreen(ScreensManager.Screens.MAIN);
    }

    public void CollectMoney()
    {
        wallet += totalGain;
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreensManager.instance.ChangeScreen(ScreensManager.Screens.MAIN);
    }

    public void CollectDoubleMoney()
    {
        wallet += totalGain * 2;
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreensManager.instance.ChangeScreen(ScreensManager.Screens.MAIN);
    }
}