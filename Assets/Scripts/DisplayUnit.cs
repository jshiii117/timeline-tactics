using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DisplayUnit : NetworkBehaviour
{
    // Start is called before the first frame update
   void Awake() {
        Debug.Log("Hello");
        System.Guid newUnitId = System.Guid.NewGuid();

        NetworkClient.RegisterPrefab(gameObject, newUnitId);

        NetworkServer.Spawn(gameObject, newUnitId);

   }
}
