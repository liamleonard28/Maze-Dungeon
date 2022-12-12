using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : Entity
{
    [SerializeField]
    GameObject throwingKnife;
    [SerializeField]
    GameObject knife;
    [SerializeField]
    float speed = 3;
    [SerializeField]
    GameObject healthBar;
    [SerializeField]
    GameObject gold;
    Rigidbody2D body;

    Melee melee;

    bool move = false;
    int moveDir = 0;
    bool turn = false;
    int turnDir = 0;

    void Awake()
    {
        //update health bar and gold display
        health = Inventory.health;
        healthBar.GetComponent<HealthBar>().set(Inventory.health);
        gold.GetComponent<TextMeshProUGUI>().text = Inventory.gold+"g";
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        knife = Instantiate(knife, transform.position, Quaternion.identity);
        knife.transform.parent = transform;
        melee = knife.GetComponent<Melee>();
    }

    // Update is called once per frame
    void Update()
    {
        //check if arrow keys / WASD pressed
        move = false;
        moveDir = 0;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)){
            move = true;
            moveDir++;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)){
            move = true;
            moveDir--;
        }

        turn = false;
        turnDir = 0;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)){
            turn = true;
            turnDir++;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)){
            turn = true;
            turnDir--;
        }

        //throw knife if left mouse button pressed
        if (Input.GetMouseButtonDown(0)){
            Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            GameObject newKnife = Instantiate(throwingKnife, transform.position, Quaternion.identity);
            newKnife.GetComponent<Projectile>().aim(target);

        }
        //swing knife if right mouse button pressed
        if (Input.GetMouseButtonDown(1)){
            Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            melee.activate(target);
        }
    }

    void FixedUpdate()
    {
        //move if arrow keys / WASD were pressed
        if (turn){
            transform.Rotate(new Vector3(0, 0, 1) * turnDir);
        }

        if (move){
            body.AddForce(transform.up * moveDir * speed);
        }
    }

    //add money to inventory and update gold display
    public void addMoney()
    {
        Inventory.gold++;
        gold.GetComponent<TextMeshProUGUI>().text = Inventory.gold+"g";
    }

    //take damage and update health bar
    public override void damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            die();
        }
        else
        {
            Inventory.health = health;
            healthBar.GetComponent<HealthBar>().set(Inventory.health);
        }   
    }

    //if die, load death scene
    public override void die()
    {
        Inventory.reset();
        SceneManager.LoadScene("Death");
    }
}
