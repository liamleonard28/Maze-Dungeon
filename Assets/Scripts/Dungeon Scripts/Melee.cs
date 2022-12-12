using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField]
    float degrees;
    [SerializeField]
    public int duration;
    [SerializeField]
    int damage=1;

    float degreesPerFrame;
    float frames = 0;

    float distance;

    bool active = false;

    [SerializeField]
    string targetTag;

    Rigidbody2D body;
    Collider2D hitbox;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();

        degreesPerFrame = degrees/duration;
        distance = (transform.parent.lossyScale.y + transform.lossyScale.y)/2;
    }

    void FixedUpdate()
    {
        //swing weapon if active
        if (active)
        {
            if (frames < duration)
            {
                transform.RotateAround(transform.parent.position, new Vector3(0,0,1), -degreesPerFrame);
                frames++;
            }
            else{
                deactivate();
            }
        }
    }

    //if enemy or object collides do damage 
    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag) || collision.gameObject.CompareTag("Object")){
            collision.gameObject.GetComponent<Entity>().damage(damage);
        }
    }

    //become visible and start collision detection
    public void activate(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction = Quaternion.Euler(0, 0, degrees/2) * direction;
        direction.Normalize();

        transform.position = transform.parent.position + direction*distance;
        transform.up = direction;

        body.simulated = true;
        hitbox.enabled = true;
        sprite.enabled = true;

        //body.angularVelocity = angularVelocity;

        active = true;
        frames=0;
    }

    //become invisible and stop collision detection
    public void deactivate(){
        body.simulated = false;
        hitbox.enabled = false;
        sprite.enabled = false;

        active = false;
    }
}
