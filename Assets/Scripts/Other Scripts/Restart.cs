using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //wait until player clicks mouse to return to dungeon
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
