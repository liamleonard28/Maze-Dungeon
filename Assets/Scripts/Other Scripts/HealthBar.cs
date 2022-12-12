using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    GameObject health;
    [SerializeField]
    GameObject[] healthDisplay = new GameObject[10];

    void Awake()
    {
        //create 10 "blocks" on health bar to represent health points
        for(int i=0; i<10; i++)
        {
            healthDisplay[i] = Instantiate(health,transform.position+new Vector3((i-4.5f)*0.069f, 0, 0), Quaternion.identity);
            healthDisplay[i].transform.parent = transform;
        }
    }

    //hide/show appropriate number of blocks to indicate health level
    public void set(int value)
    {
        for(int i=0; i<10; i++)
        {
            if (value < i)
            {
                healthDisplay[i].GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                healthDisplay[i].GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
