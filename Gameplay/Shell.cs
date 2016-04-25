using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {

    public float forceMin;
    public float forceMax;
    public float lifetime = 4f;

    Rigidbody rbody;
    float fadetime = 2f;

    void Start () {
        rbody = GetComponent<Rigidbody> ();
        float force = Random.Range (forceMin, forceMax);
        rbody.AddForce (transform.right * force);
        rbody.AddTorque (Random.insideUnitSphere * force);

        StartCoroutine (Fade ());
    }

    IEnumerator Fade () {
        yield return new WaitForSeconds (lifetime);

        float percent = 0;
        float fadeSpeed = 1 / fadetime;

        Material mat = GetComponent<Renderer> ().material;
        Color initialColor = mat.color;

        while (percent < 1) {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp (initialColor, Color.clear, percent);
            yield return null;
        }

        Destroy (gameObject);
    }

    void Update () {

    }
}
