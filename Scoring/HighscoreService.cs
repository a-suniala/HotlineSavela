using UnityEngine;
using System.Collections;

public class HighscoreService : MonoBehaviour {

    const string privateCode = "ORTvwCGARUmElXe8ynZtHwTiwIOET2pEOIcetWLZfU8A";
    const string publicCode = "56b5ca906e51b612e0743be6";
    const string webURL = "http://dreamlo.com/lb/";

    public static Highscore[] highscoresList;
    public static HighscoreService instance;
    public int scoreLimit = 3;

    public event System.Action OnHighScoreDownloaded;

    void Awake () {
        if (instance == null) instance = this;
        else if (instance != null) Destroy (gameObject);

        DownloadHighscores ();
    }

    public static void AddNewHighscore (string username, ulong score) {
        instance.StartCoroutine (instance.UploadNewHighscore (username, score));
    }

    public ulong LowestHighScore () {
        return highscoresList[instance.scoreLimit - 1].score;
    }

    IEnumerator UploadNewHighscore (string username, ulong score) {
        WWW www = new WWW (webURL + privateCode + "/add/" + WWW.EscapeURL (username) + "/" + score);
        yield return www;

        if (string.IsNullOrEmpty (www.error))
            DownloadHighscores ();
        else {
            print ("Error uploading: " + www.error);
        }
    }

    public void DownloadHighscores () {
        StartCoroutine ("DownloadHighscoresFromDatabase");
    }

    IEnumerator DownloadHighscoresFromDatabase () {
        WWW www = new WWW (webURL + publicCode + "/pipe/" + scoreLimit.ToString());
        yield return www;

        if (string.IsNullOrEmpty (www.error)) {
            FormatHighscores (www.text);
            //highscoreView.OnHighscoresDownloaded (highscoresList);
            if (OnHighScoreDownloaded != null) OnHighScoreDownloaded ();
        }
        else {
            print ("Error Downloading: " + www.error);
        }
    }

    void FormatHighscores (string textStream) {
        string[] entries = textStream.Split (new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highscoresList = new Highscore[entries.Length];

        for (int i = 0; i < entries.Length; i++) {
            string[] entryInfo = entries[i].Split (new char[] { '|' });
            string username = entryInfo[0];
            ulong score = ulong.Parse (entryInfo[1]);
            highscoresList[i] = new Highscore (username, score);
        }
    }

}

public struct Highscore {
    public string username;
    public ulong score;

    public Highscore (string _username, ulong _score) {
        username = _username;
        score = _score;
    }

}