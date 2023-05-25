using Unity.Netcode;
using UnityEngine;

public class DeathZone : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer)
        {
            return;
        }
        print("CollisionDetected");

        Destructable dest = col.GetComponentInParent<Destructable>();
        dest.GetDamage(dest.Health);
    }
}