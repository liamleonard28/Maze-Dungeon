using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{   
    [SerializeField]
    GameObject warning;
    [SerializeField]
    GameObject healthBar;
    [SerializeField]
    GameObject gold;

    void Start()
    {
        healthBar.GetComponent<HealthBar>().set(Inventory.health);
        gold.GetComponent<TextMeshProUGUI>().text = Inventory.gold+"g";
    }

    //called if player presses play draughts button
    public void playDraughts()
    {
        //if player has enough money subtract money from inventory and load draughts scene else indicate lack of money to player
        if (Inventory.pay(3))
        {
            SceneManager.LoadScene("Draughts");
        }
        else
        {
            warning.GetComponent<TextMeshProUGUI>().text = "You do not have enough Gold for that!";
        }
    }

    //called if player presses take rest button
    public void rest()
    {
        //if player has enough money subtract money from inventory and heal player else indicate lack of money to player
        if (Inventory.pay(5))
        {
            Inventory.heal(5);
            healthBar.GetComponent<HealthBar>().set(Inventory.health);
            gold.GetComponent<TextMeshProUGUI>().text = Inventory.gold+"g";
        }
        else
        {
            warning.GetComponent<TextMeshProUGUI>().text = "You do not have enough Gold for that!";
        }
    }

    //called if player presses go to dungeon button
    public void goToDungeon()
    {
        //display dungeon instructions
        SceneManager.LoadScene("Instructions");
    }
}
