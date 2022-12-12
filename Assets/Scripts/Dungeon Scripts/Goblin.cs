using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    float timeToHeal;
    float timeToReturn;

    public Vector3 home;
    public bool wantsToHide = false;

    //override generic damage function so if low on health will flee to safety
    public override void damage(int damage)
    {
        health -= damage;

        if (health <= 0){
            die();
        }
        else if (health == 1){
            flee();
        }
    }

    public override void setIdle()
    {
        timeToReturn = Time.time + Random.Range(60.0f, 90.0f);
        state = stateType.idle;
        startWander();
    }

    //check if can see player, and after a random period being idle return to home
    protected override void idle()
    {
        if (senses.canSee())
        {
            startFight();
        }
        else
        {
            if (Time.time >= timeToReturn){
                setGoal(home);
                wantsToHide = true;
            }
        }
    }

    //run to safety
    public void flee()
    {
        state = stateType.flight;
        setGoal(home);
        senses.stopLooking();
    }

    //hide in goblin hole until healed
    public void hide()
    {
        stop();
        state = stateType.inactive;
        body.simulated = false;
        hitbox.enabled = false;
        sprite.enabled = false;
        senses.stopLooking();
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("PlayerAttack"))
            {
                Destroy(child.gameObject);
            }
        }

        timeToHeal = Time.time + (3-health)*30;
    }

    //emerge from goblin hole
    public void spawn()
    {
        wantsToHide = false;
        transform.position = home;
        body.simulated = true;
        hitbox.enabled = true;
        sprite.enabled = true;
        health = 3;
        senses.startLooking();
        setIdle();
    }

    public bool isHealthy()
    {
        if (health < 3){
            return Time.time >= timeToHeal;
        }
        return true;
    }
}