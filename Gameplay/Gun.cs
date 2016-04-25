using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode;

    public Transform[] projectileSpawners;
    public Projectile projectile;
    public float msBetweenShots = 100f;
    public float muzzleVelocity = 35f;
    public int burstCount = 3;
    public int projectilesPerMag;
    public float reloadTime = .3f;

    [Header ("Recoil")]
    public Vector2 kickMinMax = new Vector2 (.05f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2 (3f, 5f);
    public float recoilMoveSettleTime = .1f;
    public float recoilRotationSettleTime = .1f;

    [Header ("Effects")]
    public Transform shell;
    public Transform shellEjectionPoint;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    Muzzleflash muzzleFlash;
    float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;
    int projectilesRemainingInMag;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotationSmoothDampVelocity;
    float recoilAngle;

    GameObject projectileParent;
    
    void Start () {
        muzzleFlash = GetComponent<Muzzleflash> ();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
        projectileParent = new GameObject ("Projectiles");
    }

    void LateUpdate () {
        // animate recoil
        transform.localPosition = Vector3.SmoothDamp (transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp (recoilAngle, 0, ref recoilRotationSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!isReloading && projectilesRemainingInMag < 1) Reload ();
    }

    void Shoot () {
        if (!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0) {
            if (fireMode == FireMode.Burst) {
                if (shotsRemainingInBurst < 1) return;
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single) {
                if (!triggerReleasedSinceLastShot) return;
            }

            foreach (Transform spawner in projectileSpawners) {
                if (projectilesRemainingInMag < 1) break;
                projectilesRemainingInMag--;
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate (projectile, spawner.position, spawner.rotation) as Projectile;
                newProjectile.transform.parent = projectileParent.transform;
                newProjectile.SetSpeed (muzzleVelocity);
            }
            GameObject s = Instantiate (shell, shellEjectionPoint.position, shellEjectionPoint.rotation) as GameObject;
            muzzleFlash.Activate ();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp (recoilAngle, 0, 30);

            AudioManager.instance.PlaySound (shootAudio, transform.position);
        }
    }

    public void Reload () {
        if (!isReloading && projectilesRemainingInMag < projectilesPerMag) {
            StartCoroutine (AnimateReload ());
            AudioManager.instance.PlaySound (reloadAudio, transform.position);
        }
        
    }

    IEnumerator AnimateReload () {
        isReloading = true;
        yield return new WaitForSeconds (.2f);

        float reloadSpeed = 1 / reloadTime;
        float percent = 0f;
        Vector3 initialRotation = transform.localEulerAngles;
        float maxReloadAngle = 20f;

        while (percent < 1) {
            percent += Time.deltaTime * reloadSpeed;

            // Parabola equation 4(-x^2 + x) to interpolate between 0 and 1
            float interpolation = (-Mathf.Pow (percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp (0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotation + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }

    public void Aim (Vector3 aimPoint) {
        if (!isReloading) transform.LookAt (aimPoint);
    }

    public void OnTriggerHold () {
        Shoot ();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease () {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
