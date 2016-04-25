using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float damage = 1f;
    public float lifetime = 3f;
    public Color trailColor;
    public LayerMask collisionMask;
    float speed = 10f;
    // To compensate raycast against enemy movement
    float movementCompensation = .1f;

    void Start () {
        Destroy (gameObject, lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere (transform.position, .1f, collisionMask);
        if (initialCollisions.Length > 0) {
            OnHitObject (initialCollisions[0], transform.position);
        }

        //GetComponent<TrailRenderer> ().material.SetColor ("_TintColor", trailColor);
    }

    void Update () {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions (moveDistance);
        transform.Translate (Vector3.forward * moveDistance);
	}

    public void SetSpeed (float newspeed) {
        speed = newspeed;
    }

    void CheckCollisions (float moveDistance) {
        Ray ray = new Ray (transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, moveDistance + movementCompensation, collisionMask, QueryTriggerInteraction.Collide)) {
            OnHitObject (hit.collider, hit.point);
        }
    }

    void OnHitObject (Collider collider, Vector3 hitPoint) {
        IDamageable damageableObject = collider.GetComponent<IDamageable> ();
        if (damageableObject != null) damageableObject.TakeHit(damage, hitPoint, transform.forward);
        Destroy (gameObject);
    }
}
