using Unity.Netcode;
using UnityEngine;

public class MachineGun : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletPlace;
    [SerializeField] private float bulletStartSpeed = 10f;
    [SerializeField] private int damage = 5;
    [SerializeField] private float fireRatePerMinute = 300;
    [SerializeField] private float bulletLifetime = 5f;

    private float lastShotTime;
    private float maxCooldown;

    private Character owner;

    private void Awake()
    {
        maxCooldown = 60f / fireRatePerMinute;
    }

    private void Update()
    {
        if (IsOwner && Input.GetMouseButton(0))
        {
            ShootServerRpc();
        }
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        if (lastShotTime > Time.time - maxCooldown)
            return;

        float bulletTravelTime = -1;
        RaycastHit2D hit = ShootRaycast();
        if (hit)
        {
            Vector2 point = hit.point;
            bulletTravelTime = (0.1f + (point - (Vector2) bulletPlace.position).magnitude) / bulletStartSpeed;

            Destructable destructable = hit.collider.GetComponentInParent<Destructable>();
            if (destructable)
            {
                print($"Destructible {destructable.name} gets {damage} damage");
                if (destructable != GetComponentInParent<Destructable>())
                    destructable.GetDamage(damage);
            }
        }

        SpawnBulletVisualClientRpc(bulletTravelTime > 0 ? bulletTravelTime : bulletLifetime);
        lastShotTime = Time.time;
    }

    private RaycastHit2D ShootRaycast()
    {
        if (!IsServer)
            return new RaycastHit2D();

        return Physics2D.Raycast(
            bulletPlace.position,
            bulletPlace.right,
            bulletLifetime * bulletStartSpeed
        );
    }

    [ClientRpc]
    void SpawnBulletVisualClientRpc(float lifetime)
    {
        Bullet bullet = Instantiate(
            bulletPrefab, bulletPlace.position, bulletPlace.rotation
        ).GetComponent<Bullet>();
        bullet.Shoot(bulletStartSpeed);
        Destroy(bullet.gameObject, lifetime);
    }
}