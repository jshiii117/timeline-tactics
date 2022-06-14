//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class StatusEffect : MonoBehaviour
//{
//    private List<Status> statusList;
//    private List<int> turnsRemaining;
//    public void Add(Status statusType)
//    {

//        if (!statusList.Contains(statusType))
//        {
//            statusList.Add(statusType);
//            turnsRemaining.Add(statusType.duration);

//        }
//        else if (statusList.Contains(statusType))
//        {
//            turnsRemaining[statusList.FindIndex(a => a.Contains(statusType)] = statusType.duration;
//        }
//    }

//    public void PostStatusEffect()
//    {
//        foreach (string status in statusList)
//        {

//        }
//    }
//}

//public class Status
//{
//    public string statusName = "burn";
//    public int duration = 2;
//    public int damage = 12;
//    public bool preMove = false;
//}


//public class Burn : Status
//{
//    public string statusName = "burn";
//    public int duration = 2;
//    public int damage = 12;
//    public bool preMove = false;
//}

//public class Bleed : Status
//{
//    public string statusName = "bleed";
//    public int duration = 1;
//    public int damage = 5;
//    public bool preMove = false;
//}





