using Unity.Netcode;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public void Shoot(float velocity)
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * velocity;
    }
}