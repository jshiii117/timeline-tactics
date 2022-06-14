using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Caveman : Unit
{

    public override void Start()
    {
        base.Start();
    }


    public override void Update()
    {
        base.Update();
    }

    public override void SpecialAttack(GameObject target)
    {
        int unitAttack = 10;
        target.GetComponent<Unit>().unitCurrentHealth -= unitAttack;
        battleManager.allUnits.Remove(target);
        battleManager.allUnits.Add(target);

        announcement = GameObject.Find("Announcement").GetComponent<Text>();
        announcement.text = ($"{unitName} used {specialAbility} on {target.GetComponent<Unit>().unitName}");

        //Ensures the unit moving after Caveman does not have its move skipped
        battleManager.turn -= 1;
        

    }
}
