using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class UnitsManager : NetworkBehaviour
{
    private Selection selectionManager;

    public GameObject caveman;
    public GameObject monk;
    public GameObject cook;
    public GameObject crusader;
    public GameObject dummy;
    public GameObject engineer;
    public GameObject medic;
    public GameObject raidBoss;
    public GameObject zealot;

    private void Start()
    {
        GameObject icaveman = Instantiate(caveman);
        GameObject imonk = Instantiate(monk);
        GameObject icook = Instantiate(cook);
        GameObject icrusader = Instantiate(crusader);
        GameObject idummy = Instantiate(dummy);
        GameObject iengineer = Instantiate(engineer);
        GameObject imedic = Instantiate(medic);
        GameObject iraidBoss = Instantiate(raidBoss);
        GameObject instantiatedZealot = Instantiate(zealot);

        NetworkServer.Spawn(icaveman);
        NetworkServer.Spawn(imonk);
        NetworkServer.Spawn(icook);
        NetworkServer.Spawn(icrusader);
        NetworkServer.Spawn(idummy);
        NetworkServer.Spawn(iengineer);
        NetworkServer.Spawn(imedic);
        NetworkServer.Spawn(iraidBoss);
        NetworkServer.Spawn(instantiatedZealot);
        



        selectionManager = GameObject.Find("ScriptManager").GetComponent<Selection>();
        selectionManager.InitializeUnits();
    }
}
