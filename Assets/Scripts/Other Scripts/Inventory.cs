using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour//Entity
{
    public static Inventory Instance;

    public static int gold = 0;
    public static int health = 10;

    private void Awake()
    {
        //retain inventory between scenes
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            //create health bar gold bar etc
        
            DontDestroyOnLoad(gameObject);
        }
    }

    //reset inventory to starting values
    public static void reset()
    {
        gold = 0;
        health = 10;
    }

    //set inventory values
    public static void set(int newGold, int newHealth)
    {
        gold = newGold;
        health = newHealth;
    }

    //if inventory contains enough money, make payment and return whether successful
    public static bool pay(int payment)
    {
        if (payment>gold)
        {
            return false;
        }
        
        gold -= payment;

        return true;
    }

    //heal a given amount up to a maximum amount
    public static void heal(int amount)
    {
        health += amount;
        if (health > 10)
        {
            health = 10;
        }
    }
}