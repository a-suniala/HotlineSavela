using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public Image fadePlane;
    public GameObject gameOverUI;
    public GameObject enterHighscoreUI;
    public Color fadeColor;
    public Text scoreUI;
    public Text gameOverScoreUI;
    public RectTransform healthBar;
    public RectTransform streakBar;
    public RectTransform waveLeftBar;

    [Header ("New Wave Banner")]
    public float bannerSpeed = 2.5f;
    public float bannerPauseTime = 1f;
    public float bannerHighestScreenPosition = 45;
    public float bannerOffScreenPosition = -170;
    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;

    [Header ("New Wave Banner Texts")]
    public string newWaveTitleTextBeginning = "- Wave ";
    public string newWaveTitleTextEnd = " -";
    public string newWaveEnemyCountTextBeginning = "Enemies: ";
    public string newWaveEnemyCountTextEnd = "";

    public string[] waveNames = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };

    Spawner spawner;
    Player player;

    ScoreKeeper score;

    void Awake () {
        spawner = FindObjectOfType<Spawner> ();
        
        spawner.OnNewWave += OnNewWave;
        player = FindObjectOfType<Player> ();
        player.OnDeath += OnGameOver;

        score = FindObjectOfType<ScoreKeeper> ();
    }

    void Update () {
        scoreUI.text = ScoreKeeper.Score.ToString ("D6");
        float healthPercent = 0f;
        if (player != null) {
            healthPercent = player.Health / player.startingHealth;
        }
        healthBar.localScale = new Vector3 (healthPercent, 1, 1);

        float streakPercent = 0f;
        if (player != null) {
            streakPercent = score.StreakLeft ();
        }
        streakBar.localScale = new Vector3 (streakPercent, 1, streakPercent);

        float wavePercent = 0f;
        if (player != null) {
            float enemyCount = spawner.CurrentWave.enemyCount;
            if (enemyCount < 0) wavePercent = 0f;
            else wavePercent = spawner.EnemiesLeftInWave () / enemyCount;
        }
        waveLeftBar.localScale = new Vector3 (wavePercent, 1, 1);
    }

    public void EnterHighscore (string value) {
        if (ScoreKeeper.Score > HighscoreService.instance.LowestHighScore ()) {
            HighscoreService.AddNewHighscore (value, ScoreKeeper.Score);
        }
        enterHighscoreUI.SetActive (false);
    }

    void OnNewWave (int waveNumber) {
        newWaveTitle.text = newWaveTitleTextBeginning + waveNames[waveNumber - 1] + newWaveTitleTextEnd;

        string enemyCountString = (spawner.CurrentWave.infinite ? "Infinite" : spawner.CurrentWave.enemyCount.ToString());
        newWaveEnemyCount.text = newWaveEnemyCountTextBeginning + enemyCountString + newWaveEnemyCountTextEnd;
        StopCoroutine ("AnimateNewWaveBanner");
        StartCoroutine ("AnimateNewWaveBanner");
    }
    
    void OnGameOver () {
        StartCoroutine (Fade(Color.clear, fadeColor, 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive (false);
        healthBar.parent.transform.gameObject.SetActive (false);
        streakBar.parent.transform.gameObject.SetActive (false);
        waveLeftBar.parent.transform.gameObject.SetActive (false);
        gameOverUI.SetActive (true);
        Cursor.visible = true;

        if (ScoreKeeper.Score > HighscoreService.instance.LowestHighScore ()) {
            enterHighscoreUI.SetActive (true);
        }
    }


    IEnumerator AnimateNewWaveBanner () {

        float animatePercent = 0f;
        int dir = 1;

        float endDelayTime = Time.time + 1 / bannerSpeed + bannerPauseTime;

        while (animatePercent >= 0) {
            animatePercent += Time.deltaTime * bannerSpeed * dir;

            if (animatePercent >= 1) {
                animatePercent = 1;
                if (Time.time > endDelayTime) dir = -1;
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp (bannerOffScreenPosition, bannerHighestScreenPosition, animatePercent);
            yield return null;
        }
    }

    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp (from, to, percent);
            yield return null;
        }
    }

    // UI Input
    public void StartNewGame () {
        SceneManager.LoadScene ("Game");
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene ("Menu");
    }
}
