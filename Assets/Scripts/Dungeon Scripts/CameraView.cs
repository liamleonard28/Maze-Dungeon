using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    // Update is called once per frame
    void Update()
    {
        //Follow player with camera
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        transform.rotation = player.transform.rotation;
    }
}
