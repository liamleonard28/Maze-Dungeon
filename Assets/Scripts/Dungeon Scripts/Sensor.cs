using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField]
    Collider2D senseBox;
    bool inRange = false;
    bool seen = false;
    float lastSeen;

    GameObject target;

    void FixedUpdate()
    {
        //if player in range check whether can see with raycast
        if (inRange){
            bool hitsTarget = raycast();
            //if player leaves sight record last time seen
            if (seen && !hitsTarget){
                lastSeen = Time.time;
            }
            seen = hitsTarget;
        }
    }

    //if player enters range begin checking if can see
    private void OnTriggerEnter2D(Collider2D collision)
    {
        target = collision.gameObject;
        inRange = true;
    }

    //if player exits range stop checking if can see
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (seen){
            lastSeen = Time.time;
        }
        inRange = false;
        seen = false;
    }

    //check if can see player with raycast
    private bool raycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.transform.position-transform.position, 1, LayerMask.GetMask("Wall", "Player"));
        return hit.collider.gameObject.CompareTag("Player");
    }

    //return whether can see player
    public bool canSee()
    {
        return seen;
    }

    //return location of seen player
    public Vector3 getTargetPosition()
    {
        return target.transform.position;
    }

    //return last time player was seen
    public float getLastSeen()
    {
        return lastSeen;
    }

    //start looking for player
    public void startLooking()
    {
        senseBox.enabled = true;
    }

    //stop looking for player
    public void stopLooking()
    {
        senseBox.enabled = false;
        inRange = false;
        seen = false;
    }
}
