using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    void OnTriggerEnter2D (Collider2D collision)
    {
        //when player walks onto coin, add gold
        collision.gameObject.GetComponent<Player>().addMoney();
        Destroy(gameObject);
    }
}
