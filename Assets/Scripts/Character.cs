using System.Collections;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Character : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> faction = new NetworkVariable<FixedString32Bytes>("");
    [SerializeField] private Transform mark;

    public FixedString32Bytes Faction => faction.Value;

    private IEnumerator Start()
    {
        if (IsOwner)
        {
            GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().Follow = transform;
            GetComponent<RespawningDestructible>().RespawnServerRpc();
        }

        while (true)
        {
            mark.GetComponent<SpriteRenderer>().enabled = IsOwner;
            yield return new WaitForSeconds(1);
        }
    }
}