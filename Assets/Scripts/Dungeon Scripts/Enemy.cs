using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected GameObject loot;
    [SerializeField]
    GameObject sensor;
    [SerializeField]
    GameObject weapon;
    
    protected Rigidbody2D body;
    protected Collider2D hitbox;
    protected Sensor senses;
    protected SpriteRenderer sprite;

    protected Melee melee;
    protected float meleeRange;

    [SerializeField]
    protected stateType state = stateType.idle;

    movementType movementMode = movementType.wander;

    List<Vector2Int> route = new List<Vector2Int>();
    Vector2 goal = new Vector2();

    float timeToWander;
    float timeToAttack;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        sensor = Instantiate(sensor, transform.position, Quaternion.identity);
        sensor.transform.parent = transform;
        senses = sensor.GetComponent<Sensor>();
        weapon = Instantiate(weapon, transform.position, Quaternion.identity);
        weapon.transform.parent = transform;
        melee = weapon.GetComponent<Melee>();
        meleeRange = transform.lossyScale.y/2 + weapon.transform.lossyScale.y + 0.15f;
    }

    void Update(){
        //act according to state
        if (state == stateType.idle)
        {
            idle();
        }
        else if (state == stateType.fight)
        {
            fight();
        }
    }

    void FixedUpdate(){
        //move according to movement mode
        if (movementMode == movementType.goal)
        {
            followRoute();
        }
        else if (movementMode == movementType.wander)
        {
            wander();
        }
    }

    //calculate route to goal using A*, each cell of maze is a node
    void findRoute(Vector2Int goalCell)
    {
        route.Clear();

        Vector2Int curCell = Maze.getCell(transform.position);

        if (curCell == goalCell)
        {
            return;
        }

        Dictionary<Vector2Int, PathValues> openList = new Dictionary<Vector2Int, PathValues>();
        Dictionary<Vector2Int, PathValues> closedList = new Dictionary<Vector2Int, PathValues>();

        PathValues sourcePathValues = new PathValues();
        openList.Add(curCell, sourcePathValues);

        while (openList.Count > 0)
        {
            int bestF = int.MaxValue;
            Vector2Int node = new Vector2Int();
            foreach(Vector2Int curNode in openList.Keys)
            {
                int f = openList[curNode].g + openList[curNode].h;
                if (f < bestF)
                {
                    bestF = f;
                    node = curNode;
                }
            }

            closedList.Add(node, openList[node]);
            openList.Remove(node);

            List<Vector2Int> neighbours = Maze.getNeighbours(node);

            foreach (Vector2Int neighbour in neighbours)
            {
                if (neighbour == goalCell)
                {
                    route.Add(goalCell);
                    Vector2Int step = node;
                    while (step != curCell)
                    {
                        route.Insert(0, step);
                        step = closedList[step].parent;
                    }
                    break;
                }
                else
                {
                    if (!closedList.ContainsKey(neighbour))
                    {
                        if (!openList.ContainsKey(neighbour))
                        {
                            PathValues neighbourPathValues = new PathValues();
                            neighbourPathValues.g = closedList[node].g + 1;
                            //use manhattan distance heuristic since cells are not connected diagonally
                            neighbourPathValues.h = Mathf.Abs(neighbour.x - goalCell.x) + Mathf.Abs(neighbour.y - goalCell.y);
                            neighbourPathValues.parent = node;

                            openList.Add(neighbour, neighbourPathValues);
                        }
                        else if (openList[neighbour].g > closedList[node].g + 1)
                        {
                            PathValues neighbourPathValues = new PathValues();
                            openList[neighbour].g = closedList[node].g + 1;
                            openList[neighbour].parent = node;
                        }
                    }
                }
            }
        }
    }

    //move towards next node in calculated route towards goal
    void followRoute()
    {
        Vector2Int curCell = Maze.getCell(transform.position);

        if (route.Count > 0)
        {
            if (curCell == route[0])
            {
                route.RemoveAt(0);
            }
            if (route.Count > 0){
                moveTowards(route[0]);
                return;
            }
        }
            
        moveTowards(goal);
    }

    //move directly towards passed location
    void moveTowards(Vector2 target){
        if (!hitbox.OverlapPoint(target))
        {
            transform.up = (Vector3)target - transform.position;
            body.AddForce(transform.up*speed);
        }
    }

    //move at random
    void wander()
    {
        //choose random place in current or neighbouring cell then wait a random amount of time and repeat
        if (Time.time >= timeToWander)
        {
            timeToWander = Time.time + Random.Range(2.0f, 10.0f);
            Vector2Int curCell = Maze.getCell(transform.position);
            List<Vector2Int> neighbours = Maze.getNeighbours(curCell);
            neighbours.Add(curCell);
            goal = neighbours[Random.Range(0, neighbours.Count)] + new Vector2(Random.Range(-0.45f, 0.45f), Random.Range(-0.45f, 0.45f));
        }

        moveTowards(goal);
    }

    //set a goal to move towards, calculate route and change movement mode to "goal"
    protected void setGoal(Vector2 newGoal)
    {

        goal = newGoal;
        Vector2Int goalCell = Maze.getCell(goal);
        findRoute(goalCell);

        movementMode = movementType.goal;
    }

    //change movement mode to "wander"
    protected void startWander()
    {
        timeToWander = Time.time;
        movementMode = movementType.wander;
    }

    //set movement mode to "stop"
    protected void stop()
    {
        movementMode = movementType.stop;
    }

    //change state to "fight"
    public virtual void startFight()
    {
        timeToAttack = Time.time;
        state = stateType.fight;
    }

    //change state to "idle"
    public virtual void setIdle()
    {
        state = stateType.idle;
        startWander();
    }

    //if can see player, move towards them and attack
    protected virtual void fight()
    {
        if (senses.canSee())
        {
            setGoal(senses.getTargetPosition());
            //if attack cooldown has passed and in range of player, make attack
            if (Time.time >= timeToAttack)
            {
                if (Vector3.Distance(transform.position, goal) <= meleeRange)
                {
                    melee.activate(goal);
                }
                timeToAttack = Time.time + (melee.duration + 1) * Time.fixedDeltaTime;
            }
        }
        else
        {
            //if haven't seen player in 30 seconds, change state to idle
            if (Time.time >= senses.getLastSeen() + 30)
            {
                setIdle();
            }
        }
    }

    //check if can see player, if so change state to fight
    protected virtual void idle()
    {
        if (senses.canSee())
        {
            startFight();
        }
    }

    //if killed drop a coin
    public override void die()
    {
        Instantiate(loot, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected enum stateType
    {
        inactive,
        idle,
        fight,
        flight,
    }

    enum movementType
    {
        stop,
        wander,
        goal,
    }
}

class PathValues
{
    public int g;
    public int h;
    public Vector2Int parent;
}
