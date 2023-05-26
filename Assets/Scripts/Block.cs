using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Block : NetworkBehaviour
{
    private NetworkVariable<int> spriteIdx = new NetworkVariable<int>();
    private NetworkVariable<bool> isDestructible = new NetworkVariable<bool>();
    private NetworkVariable<bool> shouldCollide = new NetworkVariable<bool>();
    private NetworkVariable<int> blockArmor = new NetworkVariable<int>();
    private NetworkVariable<RigidbodyType2D> rigidbodyType = new NetworkVariable<RigidbodyType2D>();


    private Rigidbody2D rb2D;
    private Bounds colliderBounds;


    private bool falling => rb2D.velocity.y < -0.01f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        colliderBounds = GetComponent<Collider2D>().bounds;

        while (true)
        {
            TilemapParser parentParser = transform.parent.GetComponent<TilemapParser>();
            if (parentParser == null)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }

            Setup(parentParser.spriteLibrary);
            yield break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!rb2D || !falling)
            return;

        Collider2D other = collision.collider;
        bool colliderIsBehind = false;
        Bounds otherBounds = other.bounds;
        if (
            otherBounds.max.x < colliderBounds.min.x - 0.01f ||
            otherBounds.min.x - 0.01f > colliderBounds.max.x ||
            otherBounds.max.y > colliderBounds.min.y
        )
        {
            return;
        }

        CharacterMovement pm = other.GetComponentInParent<CharacterMovement>();
        Destructable d = pm != null ? pm.GetComponent<Destructable>() : null;
        if (pm && d)
        {
            print("KILL");
            d.GetDamage(d.Health);
        }
    }

    public void InitData(TilemapBlockAdapter adapter, int spriteIdx)
    {
        this.spriteIdx.Value = spriteIdx;
        isDestructible.Value = adapter.isDestructible;
        shouldCollide.Value = adapter.shouldCollide;
        blockArmor.Value = adapter.blockArmor;
        rigidbodyType.Value = adapter.rigidbodyType;
    }

    private void Setup(SpriteLibrary library)
    {
        GetComponent<SpriteRenderer>().sprite = library.GetSpriteById(spriteIdx.Value);
        if (!IsServer)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Destructable>().enabled = false;
            return;
        }

        if (shouldCollide.Value)
        {
            GetComponent<Rigidbody2D>().bodyType = rigidbodyType.Value;
        }
        else
        {
            Destroy(GetComponent<NetworkRigidbody2D>());
            Destroy(GetComponent<Rigidbody2D>());
        }

        if (isDestructible.Value)
        {
            GetComponent<Destructable>().SetMaxHealth(blockArmor.Value);
        }
        else
        {
            Destroy(GetComponent<Destructable>());
        }
    }

    private void Die()
    {
    }
}