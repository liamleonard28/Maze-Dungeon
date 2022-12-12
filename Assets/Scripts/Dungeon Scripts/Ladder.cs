using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ladder : MonoBehaviour
{
    //if player is on ladder and pressed space return to menu
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
