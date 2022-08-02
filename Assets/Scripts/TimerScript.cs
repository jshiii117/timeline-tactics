using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private float indicatorTimer = 1.0f;
    [SerializeField] private float maxIndicatorTimer = 1.0f;

    [SerializeField] public Image radialIndicatorUI;

    public Battle battleManager;

    private bool isDone = false;

    void OnEnable()
    {
        Battle.OnTurnTimeout += ResetTimer;
    }

    void Update()
    {
        if (radialIndicatorUI.fillAmount > 0)
        {
            indicatorTimer -= Time.deltaTime * 0.3f;
            radialIndicatorUI.fillAmount = indicatorTimer;
        }

        if (radialIndicatorUI.fillAmount <= 0 && !battleManager.executingMove)
        {
            StartCoroutine(TimerExpired());
        }

    }

    void ResetTimer()
    {

        radialIndicatorUI.fillAmount = 1;
        indicatorTimer = maxIndicatorTimer;
        Debug.Log("Timer Reset");
    }

    IEnumerator TimerExpired()
    {
        if (!isDone && !battleManager.executingMove) //Only executes once and if a move is not currently being executed
        {
            if (battleManager.gameState == Battle.GameState.ATurn)
            {
                battleManager.UpdateAnnouncement("Time has expired for Player A");

            }
            else if (battleManager.gameState == Battle.GameState.BTurn)
            {
                battleManager.UpdateAnnouncement("Time has expired for Player B");

            }
            else
            {
                battleManager.UpdateAnnouncement("Error: timer expired but gamestate not ATurn or BTurn");
            }

            battleManager.UpdateTargetIndicators(false);
            isDone = true;

            battleManager.moveSelected = "unselected";
            battleManager.specialAttackButton.gameObject.SetActive(true);
            battleManager.attackButton.gameObject.SetActive(true);


            yield return new WaitForSeconds(2f);

            Debug.Log("Hello this is James");
            isDone = false;

            if (!battleManager.executingMove)
            {
                battleManager.UpdateTurn();
            }
        }
    }

}

