using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Remover : NetworkBehaviour
{
    [SerializeField] float LifeTime;
    [SerializeField] NetworkObject nwo;
    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(LifeTime);
        if (IsServer)
        {
            nwo.Despawn(true);
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        StartCoroutine(WaitToDestroy());
    }
}
