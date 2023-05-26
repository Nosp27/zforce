using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Transform leaderboardParent;
    [SerializeField] private GameObject rowPrefab;

    private Dictionary<ulong, ScoreUIRow> clientTrackers = new Dictionary<ulong, ScoreUIRow>(); 

    public void RegisterTracker(ScoreTracker tracker)
    {
        ScoreUIRow row = Instantiate(rowPrefab, leaderboardParent).GetComponent<ScoreUIRow>();
        row.Subscribe(tracker);
        clientTrackers[tracker.OwnerClientId] = row;
    }
    
    public void UnregisterTracker(ScoreTracker tracker)
    {
        ScoreUIRow row = clientTrackers[tracker.OwnerClientId];
        Destroy(row.gameObject);
        clientTrackers.Remove(tracker.OwnerClientId);
    }
}
