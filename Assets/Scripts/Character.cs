using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Character : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> faction = new NetworkVariable<FixedString32Bytes>("");

    public FixedString32Bytes Faction => faction.Value;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // faction.Value = OwnerClientId.ToString();
    }

    private void Start()
    {
        if (IsOwner)
        {
            GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().Follow = transform;
        }
    }
}
