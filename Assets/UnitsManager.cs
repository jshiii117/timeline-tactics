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
        Instantiate(caveman);

        Instantiate(monk);
        Instantiate(cook);
        Instantiate(crusader);
        Instantiate(dummy);
        Instantiate(engineer);
        Instantiate(medic);
        Instantiate(raidBoss);
        Instantiate(zealot);

        selectionManager = GameObject.Find("ScriptManager").GetComponent<Selection>();
        selectionManager.InitializeUnits();
    }
}
