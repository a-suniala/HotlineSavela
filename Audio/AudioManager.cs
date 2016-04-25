using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public enum AudioChannel { Master, Sfx, Music };

    public float MasterVolumePercent { get; protected set; }
    public float SfxVolumePercent { get; protected set; }
    public float MusicVolumePercent { get; protected set; }

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    public static AudioManager instance;

    Transform audioListener;
    Transform playerT;

    SoundLibrary library;

    void Awake () {

        if (instance != null) {
            Destroy (gameObject);
        }
        else if (instance == null) {
            instance = this;
        }
        
        
        DontDestroyOnLoad (gameObject);

        library = GetComponent<SoundLibrary> ();

        musicSources = new AudioSource[2];
        for (int i = 0; i < 2; i++) {
            GameObject newMusicSource = new GameObject ("Music Source " + (i + 1));
            musicSources[i] = newMusicSource.AddComponent<AudioSource> ();
            newMusicSource.transform.parent = transform;
        }
        GameObject newSfx2DSource = new GameObject ("2D Sfx Source");
        newSfx2DSource.transform.parent = transform;
        sfx2DSource = newSfx2DSource.AddComponent<AudioSource> ();

        audioListener = FindObjectOfType<AudioListener> ().transform;
        if (FindObjectOfType<Player> () != null) {
            playerT = FindObjectOfType<Player> ().transform;
        }
        
        MasterVolumePercent = PlayerPrefs.GetFloat ("Master Volume", 1);
        SfxVolumePercent = PlayerPrefs.GetFloat ("Sfx Volume", 1);
        MusicVolumePercent = PlayerPrefs.GetFloat ("Music Volume", 1);
    }

    void Update () {
        if (playerT != null) {
            audioListener.position = playerT.position;
            audioListener.rotation = playerT.rotation;
        }
    }

    public void SetVolume (float volumePercent, AudioChannel channel) {
        switch(channel) {
            case AudioChannel.Master:
                MasterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                SfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                MusicVolumePercent = volumePercent;
                break;
        }

        foreach (AudioSource s in musicSources) {
            s.volume = MusicVolumePercent * MasterVolumePercent;
        }

        PlayerPrefs.SetFloat ("Master Volume", MasterVolumePercent);
        PlayerPrefs.SetFloat ("Sfx Volume", SfxVolumePercent);
        PlayerPrefs.SetFloat ("Music Volume", MusicVolumePercent);
        PlayerPrefs.Save ();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration) {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play ();

        StartCoroutine (AnimateMusicCrossfade (fadeDuration));
    }

    public void PlaySound (AudioClip clip, Vector3 pos) {
        if (clip != null) AudioSource.PlayClipAtPoint (clip, pos, SfxVolumePercent * MasterVolumePercent);
    }

    public void PlaySound (string soundName, Vector3 pos) {
        PlaySound (library.GetClipFromName (soundName), pos);
    }

    public void PlaySound2D (string soundName) {
        sfx2DSource.PlayOneShot (library.GetClipFromName (soundName), SfxVolumePercent * MasterVolumePercent);
    }

    void OnLevelWasLoaded (int index) {
        if (playerT == null) {
            if (FindObjectOfType<Player>() != null) {
                playerT = FindObjectOfType<Player> ().transform;
            }
        }
    }

    IEnumerator AnimateMusicCrossfade(float duration) {
        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp (0, MusicVolumePercent * MasterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp (MusicVolumePercent * MasterVolumePercent, 0, percent);
            yield return null;
        }
    }
}
