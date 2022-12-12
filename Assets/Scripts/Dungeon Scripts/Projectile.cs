using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    int damage = 1;

    [SerializeField]
    string targetTag;

    Rigidbody2D body;
    Collider2D hitbox;

    void OnCollisionEnter2D (Collision2D collision)
    {
        //if projectile has hit enemy/player depending on target or object, do damage
        if (collision.gameObject.CompareTag(targetTag) || collision.gameObject.CompareTag("Object")){
            collision.gameObject.GetComponent<Entity>().damage(damage);
        }
        
        //embed projectile in wall/object/creature and deactivate physics
        body.velocity = Vector3.zero;
        transform.parent = collision.gameObject.transform;
        body.simulated = false;
        hitbox.enabled = false;
    }

    //shoot projectile in passed direction
    public void aim(Vector3 target)
    {
        body = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();

        Vector3 direction = target - transform.position;

        transform.up = direction;
        body.velocity = transform.up*speed;
    }
}
