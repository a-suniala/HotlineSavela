using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour {

    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;
    public GameObject highscoreMenuHolder;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullscreenToggle;
    public Toggle screenEffectsToggle;
    public Toggle shadowsToggle;
    public int[] screenWidths;

    int activeScreenResIndex;

    void Start () {
        InitializePrefs ();
    }

    void InitializePrefs () {
        activeScreenResIndex = PlayerPrefs.GetInt ("ScreenResIndex");
        bool isFullScreen = PlayerPrefs.GetInt ("Fullscreen") == 1;

        volumeSliders[0].value = AudioManager.instance.MasterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.SfxVolumePercent;
        volumeSliders[2].value = AudioManager.instance.MusicVolumePercent;

        for (int i = 0; i < resolutionToggles.Length; i++) {
            resolutionToggles[i].isOn = (i == activeScreenResIndex);
        }

        fullscreenToggle.isOn = isFullScreen;
        bool effectsOn = PlayerPrefs.GetInt ("ScreenEffects") == 1;
        screenEffectsToggle.isOn = effectsOn;
        screenEffectsToggle.onValueChanged.AddListener (RestartMenu);

        bool shadowsOn = PlayerPrefs.GetInt ("ShadowsOn") == 1;
        shadowsToggle.isOn = shadowsOn;
    }

    public void Play () {
        SceneManager.LoadScene ("Game");
    }

    public void Quit () {
        Application.Quit ();
    }

    public void OptionsMenu () {
        mainMenuHolder.SetActive (false);
        highscoreMenuHolder.SetActive (false);
        optionsMenuHolder.SetActive (true);
    }

    public void MainMenu () {
        mainMenuHolder.SetActive (true);
        optionsMenuHolder.SetActive (false);
        highscoreMenuHolder.SetActive (false);
    }

    public void Highscore () {
        mainMenuHolder.SetActive (false);
        optionsMenuHolder.SetActive (false);
        highscoreMenuHolder.SetActive (true);
    }

    public void SetScreenResolution (int i) {
        if (resolutionToggles[i].isOn) {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution (screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt ("ScreenResIndex", activeScreenResIndex);
            PlayerPrefs.Save ();
        }
    }

    public void SetFullScreen (bool isFullscreen) {

        foreach (Toggle t in resolutionToggles) {
            t.interactable = !isFullscreen;
        }

        if (isFullscreen) {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution (maxResolution.width, maxResolution.height, true);
        } else {
            SetScreenResolution (activeScreenResIndex);
        }
        PlayerPrefs.SetInt ("Fullscreen", (isFullscreen ? 1 : 0));
        PlayerPrefs.Save ();
    }

    public void SetMasterVolume (float value) {
        AudioManager.instance.SetVolume (value, AudioManager.AudioChannel.Master);
    }

    public void SetSfxVolume (float value) {
        AudioManager.instance.SetVolume (value, AudioManager.AudioChannel.Sfx);
    }

    public void SetMusicVolume (float value) {
        AudioManager.instance.SetVolume (value, AudioManager.AudioChannel.Music);
    }

    public void SetScreenEffects (bool effectsOn) {
        PlayerPrefs.SetInt ("ScreenEffects", (effectsOn ? 1 : 0));
        PlayerPrefs.Save ();       
    }

    public void SetShadows (bool shadowsOn) {
        PlayerPrefs.SetInt ("ShadowsOn", (shadowsOn ? 1 : 0));
        PlayerPrefs.Save ();
    }

    void RestartMenu (bool value) {
        //PlayerPrefs.Save ();
        SceneManager.LoadScene ("Menu");
    }


}
