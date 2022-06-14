using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Cook : Unit
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
        int unitAttack = 20;
        target.GetComponent<Unit>().unitCurrentHealth -= unitAttack;
        target.GetComponent<Unit>();

        battleManager.UpdateAnnouncement($"{unitName} used {specialAbility}. {target.GetComponent<Unit>().unitName} took {unitAttack}dmg from {unitName}.");

    }

    public override void Attack(GameObject target)
    {
        int unitAttack = 50;
        Unit targetScript = target.GetComponent<Unit>();
        targetScript.unitCurrentHealth -= unitAttack;

        //ApplyStatusEffect(target, "burn");
        battleManager.UpdateAnnouncement($"{unitName} used {specialAbility}. {targetScript.unitName} took {unitAttack}dmg from {unitName} and was affected with burn");
    }
}
