using System;
using Unity.Netcode;

public class ScoreTracker : NetworkBehaviour
{
    private Character player;
    public NetworkVariable<int> Kills = new NetworkVariable<int>();
    public NetworkVariable<int> Deaths = new NetworkVariable<int>();
    private ScoreUI ui;


    public override void OnNetworkSpawn()
    {
        player = GetComponent<Character>();
        player.GetComponent<RespawningDestructible>().OnKill.AddListener(OnNewDeath);
        ui = FindObjectOfType<ScoreUI>();
        ui.RegisterTracker(this);
    }

    public override void OnNetworkDespawn()
    {
        ui.UnregisterTracker(this);
    }

    void OnNewDeath()
    {
        if (!IsServer)
            return;
        Deaths.Value++;
    }
}
