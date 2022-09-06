using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Text.RegularExpressions;
using System;

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
        // foreach (GameObject playerPrefab in GameObject.FindGameObjectsWithTag("Player"))
        // {
        //     if (playerPrefab.GetComponent<NetworkIdentity>().isLocalPlayer != true)
        //     {
        //         Debug.Log("Not local player");
        //         playerPrefab.SetActive(false);
                
        //     }
        // }
        
    }

    void OnStartServer()
    {
        base.OnStartServer();
        this.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

    }

    [Command]
    public void CmdUpdateGameState(int newGameState) {
        Selection newSelectionManager = GameObject.Find("ScriptManager").GetComponent<Selection>();

        newSelectionManager.gameState = newGameState;
    }

    [Command] 
    public void CmdUpdateSelectionList(SyncList<GameObject> list, GameObject addUnit){
        
    }

    [Command]
    public void CmdUpdateSelection(String hitObject){
        Selection newSelectionManager = GameObject.Find("ScriptManager").GetComponent<Selection>();
        
        newSelectionManager.initialHitGameObject = hitObject;
    }

    [Command]
    public void CmdConfirmSelection(Vector3 displayUnitPosition){
        Debug.Log("Updating selection GameObject name");
        
        Selection newSelectionManager = GameObject.Find("ScriptManager").GetComponent<Selection>();

        try{
            newSelectionManager.gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();
            newSelectionManager.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        }catch (Exception ex){
            Debug.Log("Exception adding client authority to selectionManager: " + ex);
        }

        newSelectionManager.hitGameObjectName = newSelectionManager.initialHitGameObject;

        newSelectionManager.initialHitGameObject = "";

        CmdSpawnSelection(displayUnitPosition);
    }

    [Command(requiresAuthority = false)]
    public void CmdSpawnSelection(Vector3 displayUnitPosition){

        try{

            Selection newSelectionManager = GameObject.Find("ScriptManager").GetComponent<Selection>();
            
            string unitName = newSelectionManager.hitGameObjectName;
            unitName = Regex.Replace(unitName, @"\s", "");
            unitName = unitName.ToLower();
            

            UnitsManager unitsManager = GameObject.Find("SelectableUnits").GetComponent<UnitsManager>();

            GameObject spawnUnit = Instantiate((GameObject)unitsManager.GetType().GetField(unitName).GetValue(unitsManager));

            // GameObject spawnUnit = Instantiate(unitsManager.zealot);
            spawnUnit.transform.position = displayUnitPosition;
            spawnUnit.gameObject.tag = "SelectedUnit";


            Debug.Log("SpawnUnit IS: " + spawnUnit);
            NetworkServer.Spawn(spawnUnit);

            if(newSelectionManager.gameState == 1) {
                newSelectionManager.selectionsA.Add(spawnUnit);
            }else {
                newSelectionManager.selectionsB.Add(spawnUnit);
            }

            
        }catch (Exception ex) {
            Debug.Log($"This is not server exception: {ex}");
        }        
    }
    

}
