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
    public List<GameObject> selectionsA;
    public List<GameObject> selectionsB;

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
        

        
    }


    public override void OnStartClient()
    {
        Debug.Log("Client started");
        GameObject testObject = GameObject.Find("SelectableUnits").GetComponent<UnitsManager>().caveman;
        // CmdUpdateHitGameObject(testObject);
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
                        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject player in players){
            Debug.Log("Found a player:" + player.name);
            
            if(player.GetComponent<CanvasScript>().isLocalPlayer){
                Debug.Log("LOCAL PLAYER");
                player.GetComponent<CanvasScript>().CmdUpdateGameState(gameState);
            }
        }
        
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

    public void ConfirmSelection()
    {
        try {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (!selectionsA.Contains(hitGameObject) && !selectionsB.Contains(hitGameObject))
            {
                if (gameState == 1)
                {
                    infoText.text = ($"Player A selected {hitGameObject.GetComponent<Unit>().unitName}");
                    selectionsA.Add(hitGameObject);

                    displayPosition = A_STARTING_POS;
                    selectionCount = selectionsA.Count - 1;
                    

                    gameState = 2;
                                            
                    foreach(GameObject player in players){
                        if(player.GetComponent<CanvasScript>().isLocalPlayer){
                            player.GetComponent<CanvasScript>().CmdUpdateGameState(gameState);
                        }
                    }
                }
                else if (gameState == 2)
                {
                    infoText.text = ($"Player B selected {hitGameObject.GetComponent<Unit>().unitName}");
                    selectionsB.Add(hitGameObject);

                    displayPosition = B_STARTING_POS;
                    selectionCount = selectionsB.Count - 1;

                    gameState = 1;

                    foreach(GameObject player in players){
                        if(player.GetComponent<CanvasScript>().isLocalPlayer){
                            player.GetComponent<CanvasScript>().CmdUpdateGameState(gameState);
                         }
                    }       
                }
                Vector3 displayUnitPosition = displayPosition + new Vector3(TEAM_UNIT_GAP * selectionCount, 0, 0);         

                foreach(GameObject player in players){                        
                    if(player.GetComponent<CanvasScript>().isLocalPlayer){
                        Debug.Log("CmdConfirmSelection");
                        player.GetComponent<CanvasScript>().CmdConfirmSelection(displayUnitPosition); 
                    }
                }
            }
            else
            {
                Debug.Log("This unit has been selected. Please choose another");
            }
        }
        
        catch (UnassignedReferenceException)
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

    // [Command] 
    // void CmdUpdateHitGameObject(GameObject hitGameObject) {
    //     Debug.Log("Updating confirmed selection GameObject");
    //     confirmedGameObject = hitGameObject;
    // }

    IEnumerator APick()
    {
        if (gameState != 1) { yield break; }

        Debug.Log("Player A's turn");

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {

                hitGameObject = hit.transform.gameObject;
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                foreach(GameObject player in players){                    
                    if(player.GetComponent<CanvasScript>().isLocalPlayer){
                        Debug.Log("LOCAL PLAYER");
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
                // infoUnitPicture = Instantiate(hitGameObject, INFOPANEL_UNIT, Quaternion.identity);

            }
        }

    }

    IEnumerator BPick()
    {
        if (gameState != 2) {yield break;}

        Debug.Log("Player B's turn");

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                
                hitGameObject = hit.transform.gameObject;
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                foreach(GameObject player in players){                    
                    if(player.GetComponent<CanvasScript>().isLocalPlayer){
                        Debug.Log("LOCAL PLAYER");
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



                GameObject.Destroy(GameObject.Find("infoUnitPicture"));
                infoUnitPicture = Instantiate(hitGameObject, INFOPANEL_UNIT, Quaternion.identity);
                //Set GameObject to idle(initial frame) state
                infoUnitPicture.name = "infoUnitPicture";

            }
        }
    }

    void Complete() { 

        if (selectionsA.Count == MAX_PLAYER_UNITS && selectionsB.Count == MAX_PLAYER_UNITS && SceneManager.GetActiveScene().name != "Battle")
        {
            gameState = 4;
            Debug.Log("Selection complete. Starting battle.");
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(GameObject.Find("SelectableUnits"));
            SceneManager.LoadScene("Battle");
        }    
    }
}
