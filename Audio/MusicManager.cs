using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

    public AudioClip mainTheme;
    public AudioClip menuTheme;

    static string sceneName;

    void Start () {
        OnLevelWasLoaded (SceneManager.GetActiveScene().buildIndex);
    }

    void OnLevelWasLoaded (int sceneIndex) {
        string newSceneName = SceneManager.GetActiveScene ().name;
        if (newSceneName != sceneName || sceneName == null) {
            sceneName = newSceneName;
            Invoke ("PlayMusic", 1f);
        }
    }

    void PlayMusic () {
        AudioClip clipToPlay = null;

        if (sceneName == "Menu") {
            clipToPlay = menuTheme;
        } else if (sceneName == "Game") {
            clipToPlay = mainTheme;
        }

        if (clipToPlay != null) {
            AudioManager.instance.PlayMusic (clipToPlay, 2);
            Invoke ("PlayMusic", clipToPlay.length);
        }
    }
}
