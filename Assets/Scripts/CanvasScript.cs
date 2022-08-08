using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CanvasScript : NetworkBehaviour
{

    public GameObject monk;
    [SerializeField] Selection selectionManager;

    void Start()
    {

        GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        selectionManager = GameObject.Find("ScriptManager").GetComponent<Selection>();
        selectionManager.infoText = GameObject.Find("infoText").GetComponent<Text>();
        selectionManager.titleText = GameObject.Find("titleText").GetComponent<Text>();
        selectionManager.classText = GameObject.Find("classText").GetComponent<Text>();
        selectionManager.hpText = GameObject.Find("hpText").GetComponent<Text>();
        selectionManager.attackText = GameObject.Find("attackText").GetComponent<Text>();
        selectionManager.speedText = GameObject.Find("speedText").GetComponent<Text>();
        selectionManager.passiveText = GameObject.Find("passiveText").GetComponent<Text>();
        selectionManager.specialText = GameObject.Find("specialText").GetComponent<Text>();

        //Selection Button
        Button selectionButton = GameObject.Find("selectionButton").GetComponent<Button>();
        selectionManager.selectionButton = selectionButton;
        selectionButton.onClick.AddListener(delegate { selectionManager.ConfirmSelection(); });

        //Left and Right Button
        GameObject.Find("leftButton").GetComponent<Button>().onClick.AddListener(delegate { selectionManager.LastPage(); });
        GameObject.Find("rightButton").GetComponent<Button>().onClick.AddListener(delegate { selectionManager.NextPage(); });

    }

    private void Update()
    {
        foreach (GameObject playerPrefab in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (playerPrefab.GetComponent<NetworkIdentity>().isLocalPlayer != true)
            {
                Debug.Log("Not local player");
                playerPrefab.SetActive(false);
                
            }
        }
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
