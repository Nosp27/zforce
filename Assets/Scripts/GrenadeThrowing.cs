using Unity.Netcode;
using UnityEngine;

public class GrenadeThrowing : NetworkBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float throwForce=10;
    [SerializeField] private float throwCooldown=1;

    private float lastThrowTime;
    
    private void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.E))
        {
            SpawnGrenadeServerRpc();
        }
    }

    [ServerRpc]
    void SpawnGrenadeServerRpc()
    {
        if (lastThrowTime + throwCooldown > Time.time)
            return;
        lastThrowTime = Time.time;
        GameObject grenadeInstance = Instantiate(grenadePrefab);
        grenadeInstance.transform.position = transform.position;
        grenadeInstance.GetComponent<Rigidbody2D>().AddForce(transform.right * throwForce);
        grenadeInstance.GetComponent<NetworkObject>().Spawn(true);
    }
}
