using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;

public class RespawningDestructible : Destructable
{
    protected override void Kill()
    {
        if (!IsServer)
            return;
        OnKill.Invoke();
        Respawn();
    }

    [ServerRpc]
    public void RespawnServerRpc()
    {
        Respawn();
    }

    void Respawn()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        int idx = Random.Range(0, spawnPoints.Length);
        health.Value = maxHealth;
        GetComponent<NetworkTransform>().Teleport(
            spawnPoints[idx].transform.position, transform.rotation, transform.localScale
        );
    }
}