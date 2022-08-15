using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class UnitsManager : NetworkBehaviour
{
    private Selection selectionManager;

    #region Unit Prefabs
    public GameObject caveman;
    public GameObject monk;
    public GameObject cook;
    public GameObject crusader;
    public GameObject dummy;
    public GameObject engineer;
    public GameObject medic;
    public GameObject raidboss; //does not allow for camel case
    public GameObject zealot;
    #endregion

    #region New GameObject container 
    GameObject icaveman; 
    GameObject imonk;
    GameObject icook;
    GameObject icrusader;
    GameObject idummy;
    GameObject iengineer;
    GameObject imedic;
    GameObject iraidBoss;
    public GameObject izealot;
    #endregion

    private void Start()
    {   
        icaveman = Instantiate(caveman);
        imonk = Instantiate(monk);
        icook = Instantiate(cook);
        icrusader = Instantiate(crusader);
        idummy = Instantiate(dummy);
        iengineer = Instantiate(engineer);
        imedic = Instantiate(medic);
        iraidBoss = Instantiate(raidboss);
        izealot = Instantiate(zealot);
 



        selectionManager = GameObject.Find("ScriptManager").GetComponent<Selection>();
        selectionManager.InitializeUnits();
    }

    void OnServerConnect() {
        NetworkServer.Spawn(icaveman);
        NetworkServer.Spawn(imonk);
        NetworkServer.Spawn(icook);
        NetworkServer.Spawn(icrusader);
        NetworkServer.Spawn(idummy);
        NetworkServer.Spawn(iengineer);
        NetworkServer.Spawn(imedic);
        NetworkServer.Spawn(iraidBoss);
        NetworkServer.Spawn(izealot);
        Debug.Log("New client joined");

    }

}

