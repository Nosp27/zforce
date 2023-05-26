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

        Destructable dest = col.GetComponentInParent<Destructable>();
        if (dest)
            dest.GetDamage(dest.Health);
    }
}