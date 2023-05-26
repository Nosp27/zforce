using UnityEngine;
using UnityEngine.UI;

public class ScoreUIRow : MonoBehaviour
{
    [SerializeField] private Transform isYou;
    [SerializeField] private Text score;

    private ScoreTracker tracker;
    
    public void Subscribe(ScoreTracker tracker)
    {
        this.tracker = tracker;
        tracker.Deaths.OnValueChanged += (value, newValue) => UpdateScore();
        tracker.Kills.OnValueChanged += (value, newValue) => UpdateScore();
        UpdateScore();
    }
    
    private void UpdateScore()
    {
        isYou.gameObject.SetActive(tracker.IsOwner);
        score.text = $"#{tracker.OwnerClientId}: {tracker.Deaths.Value} Death(s)";
    }
}
