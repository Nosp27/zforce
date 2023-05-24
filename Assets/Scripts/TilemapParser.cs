using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapParser : NetworkBehaviour
{
    private Sprite[] distinctSprites;
    public Sprite[] DistinctSprites => Lazy(GetDistinctSprites, ref distinctSprites);

    [SerializeField] private GameObject blockPrefab;

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
        tms = tmGameObject.GetComponentsInChildren<Tilemap>();
        if (IsServer)
        {
            foreach (Tilemap tm in tms)
            {
                ParseTilemap(tm);
            }
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
                    SpawnBlock(col, row, t, adapter);
                }
            }
        }
    }

    private Sprite[] GetDistinctSprites()
    {
        Tilemap[] tms = tmGameObject.GetComponentsInChildren<Tilemap>();
        List<Sprite> _distinctSprites = new List<Sprite>();
        foreach (var tm in tms)
        {
            var bounds = tm.cellBounds;
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                for (int row = bounds.yMin; row < bounds.yMax; row++)
                {
                    Sprite t = tm.GetSprite(new Vector3Int(col, row, 0));
                    if (t == null)
                        continue;

                    int idx = _distinctSprites.IndexOf(t);
                    if (idx < 0)
                    {
                        _distinctSprites.Add(t);
                    }
                }
            }
        }
        print($"Call distinct sprites {_distinctSprites.Count}");
        return _distinctSprites.ToArray();
    }

    private void SpawnBlock(int x, int y, Sprite sprite, TilemapBlockAdapter adapter)
    {
        GameObject block = Instantiate(blockPrefab, transform);
        if (IsServer)
            block.GetComponent<NetworkObject>().Spawn(true);
        block.transform.parent = transform;
        block.transform.position = transform.position + new Vector3(x, y, 0);
        Block b = block.GetComponent<Block>();
        int spriteIdx = Array.IndexOf(DistinctSprites, sprite);
        b.InitData(adapter, spriteIdx);
    }
}