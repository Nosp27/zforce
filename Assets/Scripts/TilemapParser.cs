using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapParser : NetworkBehaviour
{
    public SpriteLibrary spriteLibrary { get; private set; }
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private GameObject deadZonePrefab;

    [SerializeField] private GameObject tmGameObject;
    private Tilemap[] tms;

    private T Lazy<T>(Func<T> func, ref T result)
    {
        if (result == null)
        {
            result = func();
        }

        return result;
    }
    
    public override void OnNetworkSpawn()
    {
        spriteLibrary = FindObjectOfType<SpriteLibrary>();
        tms = tmGameObject.GetComponentsInChildren<Tilemap>();
        if (IsServer)
        {
            foreach (Tilemap tm in tms)
            {
                tm.CompressBounds();
                ParseTilemap(tm);
            }
            SpawnDeadzone(tms);
        }
        else
        {
            Physics.autoSimulation = false;
        }
    }

    private void ParseTilemap(Tilemap tm)
    {
        var adapter = tm.GetComponent<TilemapBlockAdapter>();
        var bounds = tm.cellBounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            for (int row = bounds.yMin; row < bounds.yMax; row++)
            {
                Sprite t = tm.GetSprite(new Vector3Int(col, row, 0));
                if (t != null)
                {
                    if (t.name == "spawnSprite")
                    {
                        SpawnRespawn(col, row);
                    }
                    else
                    {
                        SpawnBlock(col, row, t, adapter);   
                    }
                }
            }
        }
    }

    private void SpawnDeadzone(Tilemap[] tms)
    {
        int bottom = tms.Min(x => x.cellBounds.yMin);
        int width = tms.Max(x => x.cellBounds.xMax) - tms.Min(x => x.cellBounds.xMin);
        int center = tms.Min(x => x.cellBounds.xMin) + width / 2;
        
        GameObject deadzone = Instantiate(deadZonePrefab);
        BoxCollider2D col = deadzone.GetComponent<BoxCollider2D>();
        col.size = new Vector2(width * 4, 20);
        col.offset = Vector2.zero;
        col.transform.position = new Vector3(center, bottom - 15);
        deadzone.GetComponent<NetworkObject>().Spawn(true);
    }

    private void SpawnBlock(int x, int y, Sprite sprite, TilemapBlockAdapter adapter)
    {
        GameObject block = Instantiate(blockPrefab, transform);
        if (IsServer)
            block.GetComponent<NetworkObject>().Spawn(true);
        block.transform.parent = transform;
        block.transform.position = transform.position + new Vector3(x, y, 0);
        Block b = block.GetComponent<Block>();
        int spriteIdx = spriteLibrary.GetSpriteId(sprite);
        b.InitData(adapter, spriteIdx);
    }

    private void SpawnRespawn(int x, int y)
    {
        GameObject block = Instantiate(spawnPrefab, transform);
        if (IsServer)
            block.GetComponent<NetworkObject>().Spawn(true);
        block.transform.parent = transform;
        block.transform.position = transform.position + new Vector3(x, y, 0);
    }
}