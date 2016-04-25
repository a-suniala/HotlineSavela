using UnityEngine;
using System.Collections;

public class ScreenEffectManager : MonoBehaviour {

    public Camera mainCamera;
    public string[] screenEffectTypeNames;
    public Light[] lights;

    void Start () {
        bool effectsOn = PlayerPrefs.GetInt ("ScreenEffects") == 1;
        if (!effectsOn) {
            foreach (string effect in screenEffectTypeNames) {
                Component c = mainCamera.GetComponent (effect);
                Destroy (c);
            }
        }
        bool shadowsOn = PlayerPrefs.GetInt ("ShadowsOn") == 1;
        if (!shadowsOn) SetShadows (false);
    }

    void SetShadows(bool isEnabled) {
        if (lights.Length > 0) {
            if (!isEnabled) {
                foreach (Light l in lights) {
                    l.shadows = LightShadows.None;
                }
            }
        }
    }
}
