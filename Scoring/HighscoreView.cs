using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighscoreView : MonoBehaviour {

    public Text[] highScoreNamesText;
    public Text[] highScoreScoresText;

    void Start () {
        for (int i = 0; i < highScoreNamesText.Length; i++) {
            highScoreNamesText[i].text = "Fetching...";
        }
        for (int i = 0; i < highScoreScoresText.Length; i++) {
            highScoreScoresText[i].text = "...";
        }

        HighscoreService.instance.OnHighScoreDownloaded += OnReadyToViewHighscore;

        StartCoroutine (RefreshHighscores ());
    }

    void OnDisable () {
        HighscoreService.instance.OnHighScoreDownloaded -= OnReadyToViewHighscore;
    }

    void OnReadyToViewHighscore () {
        
        Highscore[] highscoreList = HighscoreService.highscoresList;
        for (int i = 0; i < highScoreNamesText.Length; i++) {
            highScoreNamesText[i].text = "";
            highScoreScoresText[i].text = "";
            if (highscoreList.Length > i) {
                highScoreNamesText[i].text += highscoreList[i].username;
                highScoreScoresText[i].text += highscoreList[i].score;
            }
        }
    }

    IEnumerator RefreshHighscores() {
        while (true) {
            HighscoreService.instance.DownloadHighscores ();
            yield return new WaitForSeconds (30);
        }
    }

}
