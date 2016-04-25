using UnityEngine;
using System.Collections;

public class Muzzleflash : MonoBehaviour {

    public GameObject flashHolder;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;
    public float flashTime = 0.2f;

    void Start () {
        Deactivate ();
    }

    public void Activate () {
        flashHolder.SetActive (true);

        int flashSpriteIndex = Random.Range (1, flashSprites.Length);
        foreach (SpriteRenderer r in spriteRenderers) {
            r.sprite = flashSprites[flashSpriteIndex];
        }
        Invoke ("Deactivate", flashTime);
    }

    public void Deactivate () {
        flashHolder.SetActive (false);
    }
}
