using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField]
    GameObject wall;
    [SerializeField]
    GameObject goblinHole;
    [SerializeField]
    GameObject goblin;
    [SerializeField]
    GameObject ogre;
    [SerializeField]
    GameObject bat;
    [SerializeField]
    GameObject crate;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject ladder;
    [SerializeField]
    int width;
    [SerializeField]
    int height;

    static int cellsWidth;
    static int cellsHeight;

    [SerializeField]
    static bool[,] cells;
    List<GameObject> walls;

    static Vector2Int[] dirs = new Vector2Int[] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
    List<Vector2Int> occupiedCells = new List<Vector2Int>();

    void Awake()
    {
        //Generate Random maze
        cellsWidth = width*2+1;
        cellsHeight = height*2+1;

        cells = new bool[cellsWidth, cellsHeight];
        
        List<Vector2Int> roots = new List<Vector2Int>();

        Vector2Int root = new Vector2Int(Random.Range(0, width)*2+1, Random.Range(0, height)*2+1);
        roots.Add(root);
        cells[root.x, root.y] = true;

        while (roots.Count > 0)
        {
            int curRoot = Random.Range(0, roots.Count);

            List<Vector2Int> posDirs = new List<Vector2Int>();
            for (int d=0; d<dirs.Length; d++){
                Vector2Int neighbour = roots[curRoot] + dirs[d]*2;
                if (neighbour.x>0 && neighbour.y>0 && neighbour.x<cellsWidth && neighbour.y<cellsHeight){
                    if (!cells[neighbour.x, neighbour.y]){
                        posDirs.Add(dirs[d]);
                    }
                }
            }

            if (posDirs.Count > 0){
                Vector2Int dir = posDirs[Random.Range(0, posDirs.Count)];
                Vector2Int cell = roots[curRoot];

                cell += dir;
                cells[cell.x, cell.y] = true;
                cell += dir;
                cells[cell.x, cell.y] = true;
                roots.Add(cell);
            }
            else{
                roots.RemoveAt(curRoot);
            }
        }

        //remove some random walls
        
        for (int i=0; i<Random.Range(5,15); i++)
        {
            Vector2Int cell;
            do
            {
                cell = new Vector2Int(Random.Range(1, cellsWidth-1), Random.Range(1, cellsHeight-1));
            } while (cells[cell.x, cell.y] == true);
            cells[cell.x, cell.y] = true;
        }

        //Instantiate walls

        walls = new List<GameObject>();

        for(int y=0; y<cellsWidth; y++)
        {
            for(int x=0; x<cellsHeight; x++)
            {
                if (cells[x,y] == false)
                {
                    GameObject newWall = Instantiate(wall, new Vector2(x,y), Quaternion.identity);
                    newWall.transform.parent = transform;
                    walls.Add(newWall);
                }
            }
        }

        //pick random location for player and ladder and add to occupied cells
        Vector2Int spawnPoint = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        occupiedCells.Add(spawnPoint);

        player.transform.position = new Vector3(spawnPoint.x*2+1, spawnPoint.y*2+1, 0);
        ladder.transform.position = player.transform.position;

        //spawn a random number of goblin holes
        for (int i=0; i<Random.Range(1,3); i++)
        {
            //goblin hole spawn
            GameObject newGoblinHole = spawn(goblinHole);

            //spawn a random number of goblins and assign home to goblin hole
            for (int g=0; g<Random.Range(1,3); g++)
            {
                GameObject newGoblin = spawn(goblin);
                newGoblin.GetComponent<Goblin>().home = newGoblinHole.transform.position;
            }
        }

        //spawn an ogre
        spawn(ogre);

        //spawn a random number of crates
        for (int i=0; i<Random.Range(1,6); i++)
        {
            spawn(crate);
        }
    }

    //get the cell of passed world position
    public static Vector2Int getCell(Vector3 position){
        return new Vector2Int((int)Mathf.Round(position.x), (int)Mathf.Round(position.y));
    }

    //get neighbouring cells of passed cell
    public static List<Vector2Int> getNeighbours(Vector2Int cell){
        List<Vector2Int> neighbours = new List<Vector2Int>();

        for (int d=0; d<dirs.Length; d++){
            Vector2Int neighbour = cell + dirs[d];
            if (neighbour.x>0 && neighbour.y>0 && neighbour.x<cellsWidth && neighbour.y<cellsHeight){
                if (cells[neighbour.x, neighbour.y]){
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    //spawn passed gameobject in random place, add place to occupied cells, and return instatiated gameobject
    GameObject spawn(GameObject thingToSpawn)
    {
        //find a random spawnpoint not already in occupied cells
        Vector2Int spawnPoint;
        do
        {
            spawnPoint = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (occupiedCells.Contains(spawnPoint));

        occupiedCells.Add(spawnPoint);

        return Instantiate(thingToSpawn, new Vector3(spawnPoint.x*2+1, spawnPoint.y*2+1, 0), Quaternion.identity);
    }
}