using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Mirror;
using Random = System.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour
{
    //Test Commit
    [Header("Unit Info")]
    public string unitName;
    public string unitClass;
    public string unitDescription;
    public string passiveAbility;
    public string passiveAbilityDescription;
    public string specialAbility;
    public string specialAbilityDescription;    

    [Header("Unit Stats")]
    public int unitHealth;
    public int unitCurrentHealth;
    public int unitMinAttack;
    public int unitMaxAttack;
    public int unitSpeed;


    [Header("Battle Info")]
    [HideInInspector] public string attackTargets;
    [HideInInspector] public string specialAttackTargets;
    public bool movedThisRound = false;
    public bool isDead = false;
    public Text announcement;
    public Battle battleManager;
    private Animator animator;
    public bool IsMoved = false;


    //public NetworkAnimator netAnimator;

    public virtual void Start()
    {
        unitCurrentHealth = unitHealth;
    }

    public virtual void Update()
    {
        if (SceneManager.GetActiveScene().name == "Battle")
        {
            battleManager = GameObject.Find("BattleManager").GetComponent<Battle>();

            if (unitCurrentHealth <= 0)
            {
                isDead = true;
                battleManager.UpdateAnnouncement($"{unitName} has died.");
                this.gameObject.SetActive(false);
            }

            if (this.gameObject == battleManager.activeUnit)
            {
                this.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }



    }

    public virtual void Attack(GameObject target)
    {
        Random random = new Random();
        int unitAttack = random.Next(unitMinAttack, unitMaxAttack + 1);
        target.GetComponent<Unit>().unitCurrentHealth -= unitAttack;

        battleManager.UpdateAnnouncement($"{target.GetComponent<Unit>().unitName} took {unitAttack}dmg from {unitName}.");

    }

    public virtual void SpecialAttack(GameObject target)
    {
        Random random = new Random();
        int unitAttack = random.Next(unitMinAttack, unitMaxAttack + 1);
        target.GetComponent<Unit>().unitCurrentHealth -= unitAttack * 2;

        battleManager.UpdateAnnouncement($"{target.GetComponent<Unit>().unitName} took {unitAttack}dmg from {unitName}.");

    }



    //IEnumerator MoveCheck()
    //{
    //    Vector3 p1 = this.transform.position;
    //    yield return new WaitForEndOfFrame();
    //    Vector3 p2 = this.transform.position;

    //    IsMoved = (p1 != p2);
    //    // or : IsMoved = (p1 == p2);
    //}


    //public void ApplyStatusEffect(GameObject target, string statusType)
    //{
    //    Unit targetScript = target.GetComponent<Unit>();

    //    //Instantiating a new StatusEffect with type statusType
    //    StatusEffect newEffect = target.AddComponent<StatusEffect>();
    //    target.GetComponent<StatusEffect>().OnCreation("burn");
    //    targetScript.statusEffects.Add(newEffect);

    //    battleManager.UpdateAnnouncement($"{newEffect.type} was applied to {targetScript.unitName}");

    //    if (targetScript.statusEffects.Count > 3)
    //    {
    //        battleManager.UpdateAnnouncement($"{targetScript.unitName} is no longer affected by {targetScript.statusEffects[0].type}.");
    //        targetScript.statusEffects.RemoveAt(0);
    //    }
    //}

    //public void PreCheckStatusEffect()
    //{
    //    foreach (StatusEffect activeStatusEffect in statusEffects)
    //    {
    //        if (activeStatusEffect.orderType == "preMove")
    //        {
    //            activeStatusEffect.Effect(this.gameObject);
    //            activeStatusEffect.turnsRemaining -= 1;
    //            if (activeStatusEffect.turnsRemaining <= 0)
    //            {
    //                battleManager.UpdateAnnouncement($"{this.unitName} is no longer affected by {activeStatusEffect.type}.");
    //                statusEffects.Remove(activeStatusEffect);
    //            }
    //        }
    //    }
    //}

    //public void PostCheckStatusEffect()
    //{
    //    foreach (StatusEffect activeStatusEffect in statusEffects)
    //    {
    //        if (activeStatusEffect.orderType == "postMove")
    //        {
    //            activeStatusEffect.Effect(this.gameObject);
    //            activeStatusEffect.turnsRemaining -= 1;
    //            if (activeStatusEffect.turnsRemaining <= 0)
    //            {
    //                battleManager.UpdateAnnouncement($"{this.unitName} is no longer affected by {activeStatusEffect.type}.");
    //                statusEffects.Remove(activeStatusEffect);
    //            }
    //        }
    //    }
    //}
}


