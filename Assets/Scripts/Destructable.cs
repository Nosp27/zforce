using Unity.Netcode;
using UnityEngine;

public class Destructable : NetworkBehaviour
{
    [SerializeField] private float destructionDelay = 0.1f;
    [SerializeField] private int maxHealth = 10;
    private NetworkVariable<int> health = new NetworkVariable<int>(0);
    public int Health => health.Value;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            health.Value = maxHealth;
    }

    public void SetMaxHealth(int newMaxHealth, bool update = true)
    {
        maxHealth = newMaxHealth;
        if (update)
            health.Value = newMaxHealth;
    }

    public void GetDamage(int damage)
    {
        if (!IsServer)
            return;
        health.Value -= damage;
        if (health.Value <= 0)
        {
            health.Value = 0;
            DieClientRpc();
            if (destructionDelay >= 0)
                gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }

    [ClientRpc]
    void DieClientRpc()
    {
        BroadcastMessage("Die", SendMessageOptions.DontRequireReceiver);
    }
}