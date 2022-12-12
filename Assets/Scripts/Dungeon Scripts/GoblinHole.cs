using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinHole : MonoBehaviour
{
    [SerializeField]
    GameObject goblin;

    List<Goblin> goblins = new List<Goblin>();
    List<Goblin> woundedGoblins = new List<Goblin>();
    float timeToSpawn;

    // Start is called before the first frame update
    void Awake()
    {
        //add a random number of goblins inside hole
        int numGoblins = Random.Range(1,6);
        for (int i=0; i<numGoblins; i++)
        {
            goblins.Add( Instantiate(goblin, transform.position, Quaternion.identity).GetComponent<Goblin>() );
            goblins[i].home = transform.position;
            goblins[i].hide();
        }
        resetCountDown();
    }

    // Update is called once per frame
    void Update()
    {
        //if countdown to spawn has passed spawn a goblin
        if (Time.time >= timeToSpawn)
        {
            //if a wounded goblin has healed, spawn them otherwise spawn a healthy one if there are any
            if (woundedGoblins.Count > 0)
            {
                if (woundedGoblins[0].isHealthy())
                {
                woundedGoblins[0].spawn();
                woundedGoblins.RemoveAt(0);
                resetCountDown();
                }
            }
            else if (goblins.Count > 0)
            {
                goblins[0].spawn();
                goblins.RemoveAt(0);
                resetCountDown();
            }
        }
    }

    void resetCountDown()
    {
        timeToSpawn = Time.time + Random.Range(15.0f, 30.0f);
    }

    //if goblin wants to hide and has reached hole hide them and add to list
    private void OnTriggerStay2D(Collider2D collision)
    {
        Goblin newGoblin = collision.gameObject.GetComponent<Goblin>();
        if(collision.gameObject.GetComponent<Goblin>() != null)
        {
            if (newGoblin.wantsToHide)
            {
                newGoblin.hide();
                if (newGoblin.isHealthy())
                {
                    goblins.Add(newGoblin);
                }
                else
                {
                    woundedGoblins.Add(newGoblin);
                }
            }
        }
    }
}
