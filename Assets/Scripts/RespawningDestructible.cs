using UnityEngine;

public class RespawningDestructible : Destructable
{
    protected override void Kill()
    {
        if (!IsServer)
            return;
        
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        int idx = Random.Range(0, spawnPoints.Length);
        health.Value = maxHealth;
        transform.position = spawnPoints[idx].transform.position;
    }
}
