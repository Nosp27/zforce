using Unity.Netcode;
using UnityEngine;

public class TilemapBlockAdapter : MonoBehaviour
{
    public bool isDestructible = true;
    public bool shouldCollide = true;
    public int blockArmor = 5;
    public RigidbodyType2D rigidbodyType = RigidbodyType2D.Static;
}
