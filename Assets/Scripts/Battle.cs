using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Battle : NetworkBehaviour
{
    private class sort : IComparer<GameObject>
    {
        int IComparer<GameObject>.Compare(GameObject unitA, GameObject unitB)
        {
            int t1 = unitA.GetComponent<Unit>().unitSpeed;
            int t2 = unitB.GetComponent<Unit>().unitSpeed;
            return t1.CompareTo(t2);
        }
    }

    [Header("Unit Management")]
    public List<GameObject> allUnits;
    public readonly SyncList<GameObject> aUnits = new SyncList<GameObject>();
    public readonly SyncList<GameObject> bUnits = new SyncList<GameObject>();

    // public List<GameObject> aUnits;
    // public List<GameObject> bUnits;
    public List<Vector3> unitPositions;

    public enum GameState { Start, ATurn, BTurn, Transition, Complete };
    private readonly Vector3 INITIAL_BATTLE_POS = new Vector3(-4.384f, -0.503546f, 0f);
    //private readonly Vector3 INITIAL_HP_POS = new Vector3(-4.384f, -0.1f, 0f);
    private readonly float BATTLE_POS_GAP = 1.18f;

    public List<GameObject> hpSliders;
    public List<GameObject> targetIndicators;


    [Header("Battle Management")]
    public GameState gameState;
    public int round = 0;
    public int turn = 0;
    public GameObject activeUnit;
    public Unit activeUnitScript;
    public string moveSelected = "unselected";
    public string allowableTarget;
    public GameObject target;
    [SerializeField] public bool executingMove;
    private bool executeOnlyOnce = false;
         
    [Header("HUD Management")]
    public Button attackButton;
    public Button specialAttackButton;
    public Text announcement;
    [SerializeField] private Camera cam;
    [SerializeField] private Vector3 camInitialPosition;

    [Header("InfoBox")]
    public GameObject infoTarget;
    public Text titleText;
    public Text hpText;
    public Text attackText;
    public Text speedText;
    public Text statusEffectTexts;
    public Text passiveTitle;
    public Text passiveText;
    public Text specialTitle;
    public Text specialText;

    [Header("Player Management")]
    public int playerAMana = 8;
    [SerializeField] private Slider playerAManaBar;
    public int playerBMana = 8;
    [SerializeField] private Slider playerBManaBar;

    public delegate void ClickAction();
    public static event ClickAction OnTurnTimeout;

    



    void Start()
    {
        try{
            NetworkServer.Spawn(this.gameObject);
        } catch (Exception ex){
            Debug.Log("Exception spawning BattleManager: " + ex);
        }
        cam = Camera.main;
        camInitialPosition = cam.transform.position;

        //Adding in selected units from Selection scene 
        GameObject selectionManager = GameObject.Find("ScriptManager");
        aUnits.AddRange(selectionManager.GetComponent<Selection>().selectionsA);
        bUnits.AddRange(selectionManager.GetComponent<Selection>().selectionsB);
        allUnits.AddRange(aUnits);
        allUnits.AddRange(bUnits);

        //Sorts allUnits by fastest unitSpeed
        //Battle
        allUnits.Sort((IComparer<GameObject>)new sort());
        allUnits.Reverse();

        //Destroying unselected units and tagging units with player sides 
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            if (!allUnits.Contains(unit))
            {
                Destroy(unit);
            }
            else if (aUnits.Contains(unit))
            {
                unit.tag = "AUnit";
            }
            else if (bUnits.Contains(unit))
            {
                unit.tag = "BUnit";
            }
        }
        Destroy(selectionManager);

        for (int i = 0; i < 8; i++)
        {
            GameObject unit = allUnits[i];
            unitPositions.Add(INITIAL_BATTLE_POS + new Vector3((BATTLE_POS_GAP * i), 0f, 0f));
            unit.transform.position = unitPositions[i];

            GameObject hpSlider = GameObject.Find("UnitHealthBars").transform.GetChild(i).gameObject;
            hpSliders.Add(hpSlider);
            hpSlider.transform.position = unitPositions[i] + new Vector3(0f, 1f, 0f);
            hpSlider.GetComponent<Slider>().maxValue = unit.GetComponent<Unit>().unitHealth;
            hpSlider.GetComponent<Slider>().minValue = 0;
            hpSlider.GetComponent<Slider>().value = unit.GetComponent<Unit>().unitCurrentHealth;

            GameObject targetIndicator = GameObject.Find("TargetIndicators").transform.GetChild(i).gameObject;
            targetIndicators.Add(targetIndicator);
            targetIndicators[i].transform.position = unitPositions[i] + new Vector3(0f, 1.3f, 0f);
            targetIndicators[i].transform.gameObject.SetActive(false);
        }

        moveSelected = "unselected";


    }



    public void ShowUnitInformation()
    {
        if (Input.GetMouseButtonDown(1)) //Right mouse-click
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && (allUnits.Contains(hit.transform.gameObject)))
            {
                Unit info = hit.transform.gameObject.GetComponent<Unit>();

                titleText.text = ($"THE {info.unitName}").ToUpper();
                hpText.text = ($"HP: {info.unitCurrentHealth}/{info.unitHealth}");
                attackText.text = ($"ATK: {info.unitMinAttack}-{info.unitMaxAttack}");
                speedText.text = ($"SPD: {info.unitSpeed}");
                passiveTitle.text = ($"PASSIVE: {info.passiveAbility}").ToUpper();
                passiveText.text = (info.passiveAbilityDescription ?? "Placeholder Passive Ability Description");
                specialTitle.text = ($"SPECIAL: {info.specialAbility}").ToUpper();
                specialText.text = (info.specialAbilityDescription ?? "Placeholder Special Ability Description");
            }
        }
    }


    public IEnumerator SelectTarget()
    {
        if (moveSelected == "unselected" || executingMove == true) { yield break; }

        //Setting allowable target
        switch (moveSelected)
        {
            case "attack":
                switch (activeUnit.tag)
                {
                    case "AUnit":
                        switch (activeUnit.GetComponent<Unit>().attackTargets)
                        {
                            case "enemies":
                                allowableTarget = "BUnit";
                                break;

                            case "allies":
                                allowableTarget = "AUnit";
                                break;

                            case "all":
                                allowableTarget = "All";
                                break;

                        }
                        break;

                    case "BUnit":
                        switch (activeUnit.GetComponent<Unit>().attackTargets)
                        {
                            case "enemies":
                                allowableTarget = "AUnit";
                                break;

                            case "allies":
                                allowableTarget = "BUnit";
                                break;

                            case "all":
                                allowableTarget = "All";
                                break;

                        }
                        break;
                }
                break;

            case "specialAttack":
                switch (activeUnit.tag)
                {
                    case "AUnit":
                        switch (activeUnit.GetComponent<Unit>().specialAttackTargets)
                        {
                            case "enemies":
                                allowableTarget = "BUnit";
                                break;

                            case "allies":
                                allowableTarget = "AUnit";
                                break;

                            case "all":
                                allowableTarget = "All";
                                break;

                        }
                        break;

                    case "BUnit":
                        switch (activeUnit.GetComponent<Unit>().specialAttackTargets)
                        {
                            case "enemies":
                                allowableTarget = "AUnit";
                                break;

                            case "allies":
                                allowableTarget = "BUnit";
                                break;

                            case "all":
                                allowableTarget = "All";
                                break;

                        }
                        break;
                }
                break;

        }

        UpdateTargetIndicators(true);


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && (hit.transform.gameObject.tag == allowableTarget || allowableTarget == "All"))
            {
                target = hit.transform.gameObject;

                //Runs ExecuteMove()
                executingMove = true;


            }
        }

    }

    IEnumerator ExecuteMove()
    {
        if (!executeOnlyOnce) {
            if (executingMove == true)
            {
                if (activeUnit.transform.position != target.transform.position)
                {
                    var step = 5 * Time.deltaTime;
                    activeUnit.transform.position = Vector2.MoveTowards(activeUnit.transform.position, target.transform.position, step);
                    UpdateTargetIndicators(false);
                    Vector3 camPosition = cam.transform.position;

                    camPosition.x = Mathf.Lerp(camPosition.x, activeUnit.transform.position.x, 0.01f);
                    camPosition.y = Mathf.Lerp(camPosition.y, activeUnit.transform.position.y, 0.01f);
                    cam.transform.position = new Vector3(camPosition.x, camPosition.y, 0);
                    cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 1, 0.005f);


                    //if (Math.Abs(activeUnit.transform.position.x - target.transform.position.x) < 3)
                    //{
                    //    cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 1, 0.005f);
                    //}

                    if (activeUnit.transform.position.x < target.transform.position.x && activeUnit.transform.localScale.x < 0)
                    {
                        activeUnit.transform.localScale = new Vector3(activeUnit.transform.localScale.x * -1, activeUnit.transform.localScale.y, activeUnit.transform.localScale.z);
                    }
                    else if (activeUnit.transform.position.x > target.transform.position.x && activeUnit.transform.localScale.x > 0)
                    {
                        activeUnit.transform.localScale = new Vector3(activeUnit.transform.localScale.x * -1, activeUnit.transform.localScale.y, activeUnit.transform.localScale.z);

                    }
                }
                else if (activeUnit.transform.position == target.transform.position)
                {

                    if (moveSelected == "attack")
                    {
                        activeUnitScript.Attack(target);
                    }
                    else if (moveSelected == "specialAttack")
                    {   
                        activeUnitScript.SpecialAttack(target);
                        UpdateManaBar(activeUnit);
                    }

                    moveSelected = "unselected";
                    executeOnlyOnce = true;


                    try
                    {
                        activeUnit.GetComponent<Animator>().SetTrigger("isAttacking");
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex);
                    }



                    yield return new WaitForSeconds(2f);
                    executeOnlyOnce = false;
                    //activeUnitScript.movedThisRound = true;
                    //specialAttackButton.gameObject.SetActive(true);
                    //attackButton.gameObject.SetActive(true);
                    //allowableTarget = null;
                    //target = null;

                    //executingMove = false;

                    UpdateTurn(); // executingMove also set to false in here 



                }
            }
        }
    }

    void Update()
    {

        activeUnit = allUnits[turn];
        activeUnitScript = allUnits[turn].GetComponent<Unit>();
        activeUnit.transform.gameObject.GetComponent<Animator>().SetBool("isActive", true);

        //All Unit functions 
        UpdateUnitPosition();
        UpdateUnitAnimation();
        UpdateHealthBar();
        UpdateHUD();


        if (!executingMove && target == null)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 3, 0.01f);

            Vector3 camPosition = cam.transform.position;

            camPosition.x = Mathf.Lerp(camPosition.x, camInitialPosition.x, 0.01f);
            camPosition.y = Mathf.Lerp(camPosition.y, camInitialPosition.y, 0.01f);
            cam.transform.position = new Vector3(camPosition.x, camPosition.y, 0);
            //cam.transform.position = camInitialPosition;
        }

        if (activeUnitScript.movedThisRound && !executingMove) { Debug.Log("Updating turn because this unit has already moved this turn"); UpdateTurn(); }



        //Active Unit functions 
        ShowUnitInformation();
        StartCoroutine(SelectTarget());
        StartCoroutine(ExecuteMove());

    }
    #region Update Functions 
    void UpdateUnitPosition()
    {
        if (target != null) { return; }
        for (int i = 0; i < allUnits.Count; i++)
        {
            //allUnits[i].transform.position = unitPositions[i];
            var step = 5 * Time.deltaTime;
            allUnits[i].transform.position = Vector2.MoveTowards(allUnits[i].transform.position, unitPositions[i], step);

            if(allUnits[i].GetComponent<Unit>().IsMoved)
            {
                if (allUnits[i].transform.position.x > 0)
                {
                    allUnits[i].transform.localScale = new Vector3(-0.9868129f, 0.9868129f, 0.9868129f);
                }
                else
                {
                    allUnits[i].transform.localScale = new Vector3(0.9868129f, 0.9868129f, 0.9868129f);
                }
            }
        }
    }
    void UpdateUnitAnimation()
    {
        foreach (GameObject unit in allUnits)
        {
            if (activeUnit != unit)
            {
                unit.GetComponent<Animator>().SetBool("isActive", false);
            }
        }/* else if the unit is target, then play hurt animation*/
    }
    void UpdateHealthBar()
    {
        for (int i = 0; i < allUnits.Count; i++)
        {
            hpSliders[i].transform.position = unitPositions[i] + new Vector3(0f, 1f, 0f);
            hpSliders[i].GetComponent<Slider>().value = allUnits[i].GetComponent<Unit>().unitCurrentHealth;
        }
    }
    void UpdateHUD()
    {
        specialAttackButton.GetComponentInChildren<Text>().text = activeUnit.GetComponent<Unit>().specialAbility;
    }
    public void UpdateTurn()
    {

        executingMove = false;
        activeUnitScript.movedThisRound = true;
        specialAttackButton.gameObject.SetActive(true);
        attackButton.gameObject.SetActive(true);
        allowableTarget = null;
        target = null;
        for (int i = 0; i < allUnits.Count; i++)
        {
            //CheckDead
            if (allUnits[i].GetComponent<Unit>().isDead)
            {           
                Destroy(targetIndicators[i]);
                targetIndicators.RemoveAt(i);

                Destroy(hpSliders[i]);
                hpSliders.RemoveAt(i);

                unitPositions.RemoveAt(i);

                try
                {
                    aUnits.RemoveAt(aUnits.IndexOf(allUnits[i]));

                }catch
                {
                    bUnits.RemoveAt(bUnits.IndexOf(allUnits[i]));
                }

                Destroy(allUnits[i]);
                allUnits.RemoveAt(i);


            }
        }

            if (turn != (allUnits.Count - 1))
        {
            turn++; 
        }
        else
        {
            round++;
            turn = 0;
            announcement.text = ($"Round {round}");
            foreach (GameObject unit in allUnits)
            {
                unit.GetComponent<Unit>().movedThisRound = false;
            }
        }

        if (activeUnit.tag == "AUnit")
        {
            gameState = GameState.ATurn;
            UpdateAnnouncement("Player A's Turn");

        }
        else
        {
            gameState = GameState.BTurn;
            UpdateAnnouncement("Player B's Turn");
        }

        OnTurnTimeout(); //Resets timer
    }
    public void UpdateTargetIndicators(bool show)
    {
        if (show)
        {
            try {
                switch (allowableTarget)
                {


                    case "BUnit":
                        foreach (GameObject unit in bUnits)
                        {
                            targetIndicators[allUnits.IndexOf(unit)].SetActive(true);
                        }
                        break;
                    case "AUnit":
                        foreach (GameObject unit in aUnits)
                        {
                            targetIndicators[allUnits.IndexOf(unit)].SetActive(true);
                        }
                        break;
                    case "All":
                        foreach (GameObject targetIndicator in targetIndicators)
                        {
                            targetIndicator.SetActive(true);
                        }
                        break;
                }
            } catch (Exception ex) {
                Debug.Log($"Exception in showing indicator: {ex}");

            }
        }
        else
        {
            foreach (GameObject targetIndicator in targetIndicators)
            {
                targetIndicator.SetActive(false);
            }
        }
    }

    public void UpdateAnnouncement(string message)
    {
        announcement.text = message;

        switch (gameState)
        {
            case GameState.ATurn:
                announcement.color = new Color32(4, 148, 52, 255);
                break;

            case GameState.BTurn:
                announcement.color = new Color32(138, 3, 25, 255);
                break;

            default:
                announcement.color = Color.black;
                break;
        }

    }

    void UpdateManaBar(GameObject currentUnit)
    {
        if (aUnits.Contains(currentUnit)) 
        {
                playerAMana -= 1;
                playerAManaBar.value -= 0.125f;
        }
        else if (bUnits.Contains(currentUnit)) 
        {
            playerBMana -= 1;
            playerBManaBar.value -= 0.125f;

        }
        else 
        { 
            Debug.Log("Error with UpdateManaBar"); 
        }
    }
    #endregion

    #region HUD Button Functions 
    public void OnAttackButtonPress()
    {
        moveSelected = "attack";
        specialAttackButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(false);
    }
    public void OnSpecialAttackButtonPress() //If timer expires after move is selected mana is not refunded 
        //Mana should be expended when move is finished not when selected
    {
        if (gameState == GameState.ATurn)
        {
            if (playerAMana > 0)
            {
                moveSelected = "specialAttack";
                specialAttackButton.gameObject.SetActive(false);
                attackButton.gameObject.SetActive(false);
                //UpdateManaBar(GameState.ATurn);
            }
            else
            {
                announcement.text = "Player A not enough mana";
            }
        }

        if (gameState == GameState.BTurn)
        {
            if (playerBMana > 0)
            {
                moveSelected = "specialAttack";
                specialAttackButton.gameObject.SetActive(false);
                attackButton.gameObject.SetActive(false);
                //UpdateManaBar(GameState.BTurn);
            }
            else
            {
                announcement.text = "Player B not enough mana";
            }
        }

    }
    #endregion
}

public class Hello : MonoBehaviour
{

}
