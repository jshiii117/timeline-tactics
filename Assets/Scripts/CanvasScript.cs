using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CanvasScript : NetworkBehaviour
{

    public GameObject monk;

    void Start()
    {

        this.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
    }

    private void Update()
    {
        //this.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //this.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //Debug.Log("Is this working lol");

    }

    void OnStartServer()
    {
        base.OnStartServer();
        this.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

    }

    public void OnButtonPress()
    {
        Instantiate(monk); 

    }

    
    

}
