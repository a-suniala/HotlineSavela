using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity {

    public float moveSpeed = 5f;

    public Crosshair crossHair;

    public float minAimCursorDistance = 1f;

    public float dyingYDepthThreshold = -10f;

    Camera mainCamera;
    PlayerController controller;
    GunController gun;

    void Awake () {
        controller = GetComponent<PlayerController> ();
        gun = GetComponent<GunController> ();
        mainCamera = Camera.main;
        FindObjectOfType<Spawner> ().OnNewWave += OnNewWave;
    }

	protected override void Start () {
        base.Start ();
        
	}
	
	void Update () {
        Movement ();
        LookRotate ();

        // Gun handling
        if (Input.GetButton("Fire1")) {
            gun.OnTriggerHold ();
        }
        if (Input.GetButtonUp ("Fire1")) {
            gun.OnTriggerRelease ();
        }
        if (Input.GetButtonDown("Reload")) {
            gun.Reload ();
        }

        if (transform.position.y < dyingYDepthThreshold) {
            TakeDamage (Health);
        }
    }

    protected override void Die () {
        AudioManager.instance.PlaySound ("PlayerDeath", transform.position);
        base.Die ();
    }

    void OnNewWave (int waveNumber) {
        Health = startingHealth;
        gun.EquipGun (waveNumber - 1);
    }

    void Movement () {
        Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move (moveVelocity);
    }

    void LookRotate () {
        Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);
        Plane groundPlane = new Plane (Vector3.up, Vector3.up * gun.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast (ray, out rayDistance)) {
            Vector3 point = ray.GetPoint (rayDistance);
            Debug.DrawLine (ray.origin, point, Color.red);
            controller.LookAt (point);
            crossHair.transform.position = point;
            crossHair.TargetDetect (ray);
            if ((new Vector2 (point.x, point.z) - new Vector2 (transform.position.x, transform.position.z)).sqrMagnitude > Mathf.Pow(minAimCursorDistance, 2)) {
                gun.Aim (point);
            }
        }
    }
}
