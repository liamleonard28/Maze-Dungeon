using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Entity
{
    [SerializeField]
    GameObject loot;

    public override void die()
    {
        //drop coin when destroyed
        Instantiate(loot, transform.position, Quaternion.identity);
        Destroy(gameObject);//replace
    }
}
