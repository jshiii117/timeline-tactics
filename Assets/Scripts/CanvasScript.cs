using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CanvasScript : MonoBehaviour
{

    public GameObject monk;

    void Start()
    {

        this.GetComponent<Canvas>().worldCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        
    }

    public void OnButtonPress()
    {
        Instantiate(monk); 

    }

    
    

}
