using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    protected int health;

    public virtual void damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            die();
        }
    }

    public int getHealth()
    {
        return health;
    }

    public virtual void die()
    {
        Destroy(gameObject);
    }
}
