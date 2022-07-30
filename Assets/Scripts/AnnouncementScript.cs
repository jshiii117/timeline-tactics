using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncementScript : MonoBehaviour
{

    public Camera camera;
    //[SerializeField] private float cameraY;

    void Update()
    {
        //cameraY = camera.transform.position.y;
        this.transform.position = camera.transform.position + new Vector3(0, 1.6f, 0);
    }
}
