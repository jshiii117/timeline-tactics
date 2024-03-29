using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class Selection : NetworkBehaviour
{
    // public enum GameState { Start, APick, BPick, Transition, Complete };

    [SyncVar]
    public int gameState;
    // 0 = start, 1 = A, 2 = B, 3 = transition, 4 = complete

    [Header("Unit Selections")]
    public List<GameObject> selectionAll;
    public readonly SyncList<GameObject> selectionsA = new SyncList<GameObject>();
    public readonly SyncList<GameObject> selectionsB = new SyncList<GameObject>();

    [Header("Info Panel")]
    public Text titleText;
    public GameObject infoUnitPicture;
    public Text infoText;
    public Text classText;
    public Text hpText;
    public Text attackText;

    public Text speedText;
    public Text passiveText;
    public Text specialText;
    public Button selectionButton;

    private int page;
    public List<GameObject> shownUnits;

    private readonly int MAX_PLAYER_UNITS = 4;

    private readonly Vector3 TOP_ROW_STARTING_POS = new Vector3(-2.997f, 0.512f, 0f);
    private readonly Vector3 BOT_ROW_STARTING_POS = new Vector3(-2.997f, -0.488f, 0f);
    private readonly float SELECTION_UNIT_GAP = 1.497f; 

    private readonly Vector3 A_STARTING_POS = new Vector3(-3.083f, -1.782f, 0f);
    private readonly Vector3 B_STARTING_POS = new Vector3(-3.083f, 1.844f, 0f);
    private readonly float TEAM_UNIT_GAP = 1.502f;

    private readonly Vector3 OFFSCREEN_UNITS = new Vector3(-4.5f, -3.5f, 0);
    private readonly Vector3 INFOPANEL_UNIT = new Vector3(3.62f, 0.888f, 0);

    // [SyncVar]
    public GameObject hitGameObject = null;

    [SyncVar] 
    public String initialHitGameObject;

    [SyncVar]
    public String hitGameObjectName;


    Vector3 displayPosition;
    int selectionCount;



    void Start()
    {
        infoUnitPicture = new GameObject();
        infoUnitPicture.AddComponent<SpriteRenderer>();
        infoUnitPicture.layer = 5;
        infoUnitPicture.name = "Info Unit Picture";
        infoUnitPicture.transform.position = INFOPANEL_UNIT;
        infoUnitPicture.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        infoUnitPicture.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }


    public override void OnStartClient()
    {
        GameObject testObject = GameObject.Find("SelectableUnits").GetComponent<UnitsManager>().caveman;
    }

    public void InitializeUnits() //Being called from UnitsManager
    {
        gameState = 0;
        selectionAll.AddRange(GameObject.FindGameObjectsWithTag("Unit"));
        foreach (GameObject unit in selectionAll)
        {
            unit.GetComponent<Animator>().SetBool("isActive", true);
        }

        for (int i = 0; i < 8; i++)
        {
            try
            {
                shownUnits.Add(selectionAll[i]);
            }
            catch (Exception e)
            {
                Debug.Log("Could not add into shownUnit" + e);
            }


        }
        UpdateShownUnits();

        gameState = 1;        
    }

    public void UpdateShownUnits()
    {
        for (int i = 0; i < 4; i++)
        {
            try
            {
                shownUnits[i].transform.position = TOP_ROW_STARTING_POS + new Vector3((SELECTION_UNIT_GAP * i), 0f, 0f);
            }
            catch (Exception e)
            {
                Debug.Log("Could not add units: " + e);
            }
        }

        for (int i = 4; i < 8; i++)
        {
            try 
            {
                shownUnits[i].transform.position = BOT_ROW_STARTING_POS + new Vector3((SELECTION_UNIT_GAP * (i - 4)), 0f, 0f);

            }
            catch (Exception e)
            {
                Debug.Log("Could not add units: " + e);
            }
        }

        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            if (!shownUnits.Contains(unit))
            {
                unit.transform.position = OFFSCREEN_UNITS;
            }
        }
    }
    public void NextPage()
    {
        if (page == 2) { page = 0; } else { page += 1; }
        shownUnits.Clear();

        try
        {
            for (int i = 0; i < 8; i++)
            {
                shownUnits.Add(selectionAll[i + (page * 8)]);
            }
        }
        catch { }
        UpdateShownUnits();

    }
    public void LastPage()
    {
        if (page == 0) { page = 2; } else { page -= 1; }
        shownUnits.Clear();
        try
        {
            for (int i = 0; i < 8; i++)
            {
                shownUnits.Add(selectionAll[i + (page * 8)]);
            }
        }
        catch { }

        UpdateShownUnits();

    }

    private bool CheckIfSelected(){
        List<GameObject> allSelections = new List<GameObject>();
        allSelections.AddRange(selectionsA);
        allSelections.AddRange(selectionsB);

        foreach(GameObject selectedUnit in allSelections){
            if (selectedUnit.GetComponent<Unit>().unitName == initialHitGameObject){
                return true;
            }
        }

        return false;
    }


    public void ConfirmSelection()
    {


        if(initialHitGameObject != ""){
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (!CheckIfSelected())
            {
                if (gameState == 1)
                {
                    infoText.text = ($"Player A selected {hitGameObject.GetComponent<Unit>().unitName}");

                    displayPosition = A_STARTING_POS;
                    selectionCount = selectionsA.Count;
                }
                else if (gameState == 2)
                {
                    infoText.text = ($"Player B selected {hitGameObject.GetComponent<Unit>().unitName}");

                    displayPosition = B_STARTING_POS;
                    selectionCount = selectionsB.Count;  
                }

                Vector3 displayUnitPosition = displayPosition + new Vector3(TEAM_UNIT_GAP * selectionCount, 0, 0);         

                foreach(GameObject player in players){                        
                    if(player.GetComponent<CanvasScript>().isLocalPlayer){
                        player.GetComponent<CanvasScript>().CmdConfirmSelection(displayUnitPosition); 
                    }
                }

                foreach(GameObject player in players){
                    if(player.GetComponent<CanvasScript>().isLocalPlayer){
                        if (gameState == 1){
                            player.GetComponent<CanvasScript>().CmdUpdateGameState(2);
                        }else if (gameState ==2) {
                            player.GetComponent<CanvasScript>().CmdUpdateGameState(1);
                        }
                    }
                }     
            }
            else
            {
                Debug.Log("This unit has been selected. Please choose another");
            }
        }
        else 
        {
            Debug.Log("Please select a unit");
        }
        
    }

    void Update()
    {
        StartCoroutine(APick());
        StartCoroutine(BPick());
        Complete();
 

    }

    IEnumerator APick()
    {
        if (gameState != 1) { yield break; }


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {

                hitGameObject = hit.transform.gameObject;
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                foreach(GameObject player in players){                    
                    if(player.GetComponent<CanvasScript>().isLocalPlayer){
                        player.GetComponent<CanvasScript>().CmdUpdateSelection(hitGameObject.GetComponent<Unit>().unitName); 
                    }
                }


                titleText.text = ($"THE {hitGameObject.GetComponent<Unit>().unitName}").ToUpper();
                infoText.text = hitGameObject.GetComponent<Unit>().unitDescription;
                classText.text = ($"CLASS: {hitGameObject.GetComponent<Unit>().unitClass}");
                hpText.text = ($"HP: {hitGameObject.GetComponent<Unit>().unitHealth}");
                attackText.text = ($"ATK: {hitGameObject.GetComponent<Unit>().unitMinAttack}-{hitGameObject.GetComponent<Unit>().unitMaxAttack}");
                speedText.text = ($"SPD: {hitGameObject.GetComponent<Unit>().unitSpeed}");
                passiveText.text = ($"PASSIVE:@{hitGameObject.GetComponent<Unit>().passiveAbility}").Replace("@", System.Environment.NewLine).ToUpper();
                specialText.text = ($"SPECIAL:@{hitGameObject.GetComponent<Unit>().specialAbility}").Replace("@", System.Environment.NewLine).ToUpper();

                infoUnitPicture.GetComponent<SpriteRenderer>().sprite = hitGameObject.GetComponent<SpriteRenderer>().sprite;

            }
        }

    }

    IEnumerator BPick()
    {
        if (gameState != 2) {yield break;}


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                
                hitGameObject = hit.transform.gameObject;
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                foreach(GameObject player in players){                    
                    if(player.GetComponent<CanvasScript>().isLocalPlayer){
                        player.GetComponent<CanvasScript>().CmdUpdateSelection(hitGameObject.GetComponent<Unit>().unitName); 
                    }
                }

                titleText.text = ($"THE {hitGameObject.GetComponent<Unit>().unitName}").ToUpper();
                infoText.text = hitGameObject.GetComponent<Unit>().unitDescription;
                classText.text = ($"CLASS: {hitGameObject.GetComponent<Unit>().unitClass}");    
                hpText.text = ($"HP: {hitGameObject.GetComponent<Unit>().unitHealth}");
                attackText.text = ($"ATK: {hitGameObject.GetComponent<Unit>().unitMinAttack}-{hitGameObject.GetComponent<Unit>().unitMaxAttack}");
                speedText.text = ($"SPD: {hitGameObject.GetComponent<Unit>().unitSpeed}");
                passiveText.text = ($"PASSIVE:@{hitGameObject.GetComponent<Unit>().passiveAbility}").Replace("@", System.Environment.NewLine).ToUpper();
                specialText.text = ($"SPECIAL:@{hitGameObject.GetComponent<Unit>().specialAbility}").Replace("@", System.Environment.NewLine).ToUpper();

                infoUnitPicture.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
                infoUnitPicture.GetComponent<SpriteRenderer>().sortingOrder = 2;
                infoUnitPicture.GetComponent<SpriteRenderer>().sprite = hitGameObject.GetComponent<SpriteRenderer>().sprite;

            }
        }
    }

    void Complete() { 

        if (selectionsA.Count == MAX_PLAYER_UNITS && selectionsB.Count == MAX_PLAYER_UNITS && SceneManager.GetActiveScene().name != "Battle")
        {
            gameState = 4;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(GameObject.Find("SelectableUnits"));
            DontDestroyOnLoad(GameObject.Find("ScriptManager"));
            //Maintaining player prefab
            DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Player"));

            foreach(GameObject selectedUnit in selectionsA){
                DontDestroyOnLoad(selectedUnit);
            }

            foreach(GameObject selectedUnit in selectionsB){
                DontDestroyOnLoad(selectedUnit);
            }
            SceneManager.LoadScene("Battle");
        }    

        if (SceneManager.GetActiveScene().name == "Battle"){
            try{
                GameObject.FindGameObjectWithTag("Player").GetComponent<CanvasScript>().InitializeBattleManager();
            }catch (Exception ex){
                Debug.Log("Exception with InitializeBattleManager: " + ex);
            }
         
            Debug.Log("Initialized Battle Manager");
        }
    }
}
