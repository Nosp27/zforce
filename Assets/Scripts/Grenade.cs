using Unity.Netcode;
using UnityEngine;

public class Grenade : NetworkBehaviour
{
    [SerializeField] private float ttl = 10f;
    [SerializeField] private float ttlAfterCollision = 2f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float maxExplosionDamage = 30f;
    [SerializeField] private float minExplosionDamage = 3f;
    [SerializeField] private GameObject explosionParticlesPrefab;
    private bool exploded;

    private bool collisionProcessed;
    private float explosionTime;

    public override void OnNetworkSpawn()
    {
        print("Grenade init");
        explosionTime = Time.time + ttl;
    }

    void Update()
    {
        if (!IsServer)
            return;

        if (!exploded && explosionTime < Time.time)
        {
            print("EXPLoDE");
            exploded = true;
            Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!collisionProcessed && col.gameObject.GetComponent<Block>() != null)
        {
            collisionProcessed = true;
            explosionTime = Time.time + ttlAfterCollision;
        }
    }

    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in colliders)
        {
            Destructable destructable = col.GetComponentInParent<Destructable>();
            
            if (!destructable)
                continue;

            float distance = (destructable.transform.position - transform.position).magnitude;
            int damage = (int)Mathf.Lerp(maxExplosionDamage, minExplosionDamage, distance / explosionRadius);
            destructable.GetDamage(damage);
        }
        
        ExplodeClientRpc();
        GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc]
    void ExplodeClientRpc()
    {
        if (explosionParticlesPrefab)
            Instantiate(explosionParticlesPrefab, transform.position, transform.rotation);
    }
}
