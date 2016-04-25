using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

    public Color highlightColor = Color.red;
    public float rotateSpeed = -40f;
    public LayerMask targetMask;

    SpriteRenderer dot;
    Color originalDotColor;

    void Start () {
        Cursor.visible = false;
        dot = transform.Find ("Dot").GetComponent<SpriteRenderer> ();
        originalDotColor = dot.color;
    }

    void Update () {
        transform.Rotate (Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    public void TargetDetect(Ray ray) {
        if (Physics.Raycast(ray, 100, targetMask)) {
            dot.color = highlightColor;
        } else {
            dot.color = originalDotColor;
        }
    }
}
