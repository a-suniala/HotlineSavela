using UnityEngine;
using System.Collections;

public class ScoreKeeper : MonoBehaviour {

    public static ulong Score { get; private set; }
    public ulong LowestHighScore { get; private set; }

    public int basisScorePerEnemy = 5;
    public float streakExpiryTime = 1f;

    float lastEnemyKilledTime;
    int streakCount;

    static ScoreKeeper instance;

    void Start () {
        if (instance == null) instance = this;
        else if (instance != null) Destroy (gameObject);

        Score = 0;
        Enemy.OnDeathStatic += OnEnemyKilled;
        if (FindObjectOfType<Player> ()) FindObjectOfType<Player> ().OnDeath += OnPlayerDeath;
    }

    public float StreakLeft () {
        return Mathf.Max (0, lastEnemyKilledTime + streakExpiryTime - Time.time) / streakExpiryTime;
    }

    void OnEnemyKilled () {
        if (Time.time < lastEnemyKilledTime + streakExpiryTime) {
            streakCount++;
        }
        else streakCount = 0;

        lastEnemyKilledTime = Time.time;

        Score += (uint)basisScorePerEnemy + (ulong)Mathf.Pow(streakCount, 3);
    }

    void OnPlayerDeath () {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
