using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State { Idle, Chase, Attack };
    State currentState;

    public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;

    public float pathRefreshRate = 0.25f;
    public float attackDistanceThreshold = .5f;
    public float timeBetweenAttacks = 1f;
    public float damage = 1f;

    NavMeshAgent pathFinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;
    Color originalColor;

    float nextAttackTime;

    float selfCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    void Awake () {
        pathFinder = GetComponent<NavMeshAgent> ();

        if (GameObject.FindGameObjectWithTag ("Player") != null) {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag ("Player").transform;
            targetEntity = target.GetComponent<LivingEntity> ();

            selfCollisionRadius = GetComponent<CapsuleCollider> ().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;
        }
    }

	protected override void Start () {
        base.Start ();

        if (hasTarget) {
            currentState = State.Chase;
            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine (UpdatePath ());
        }
        
	}

    public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection) {
        AudioManager.instance.PlaySound ("Impact", transform.position);
        if (damage >= Health && !dead) {
            if (OnDeathStatic != null) OnDeathStatic ();
            AudioManager.instance.PlaySound ("EnemyDeath", transform.position);
            Destroy (Instantiate (deathEffect.gameObject, hitPoint, Quaternion.FromToRotation (Vector3.forward, hitDirection)) 
                as GameObject, deathEffect.startLifetime);

        }
        base.TakeHit (damage, hitPoint, hitDirection);
    }

    void Update () {
        if (hasTarget && Time.time > nextAttackTime) {
            float sqrDistToTarget = (target.position - transform.position).sqrMagnitude;
            if (sqrDistToTarget < Mathf.Pow (attackDistanceThreshold + selfCollisionRadius + targetCollisionRadius, 2)) {
                nextAttackTime = Time.time + timeBetweenAttacks;
                AudioManager.instance.PlaySound ("EnemyAttack", transform.position);
                StartCoroutine (Attack ());
            }
        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor) {
        pathFinder.speed = moveSpeed;
        if (hasTarget) {
            damage = Mathf.Ceil (targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;

        deathEffect.startColor = new Color (skinColor.r, skinColor.g, skinColor.b, 1);
        skinMaterial = GetComponent<Renderer> ().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }

    void OnTargetDeath () {
        hasTarget = false;
        currentState = State.Idle;
    }

    IEnumerator Attack () {
        currentState = State.Attack;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * selfCollisionRadius;

        float percent = 0;
        float attackMoveSpeed = 3;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1) {
            if (percent >= .5f && !hasAppliedDamage) {
                hasAppliedDamage = true;
                targetEntity.TakeDamage (damage);
            }
            percent += Time.deltaTime * attackMoveSpeed;
            // Parabola equation 4(-x^2 + x) to interpolate between 0 and 1
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp (originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chase;
    }

    IEnumerator UpdatePath() {
        while (hasTarget) {
            if (currentState == State.Chase) {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * 
                    (selfCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead) pathFinder.SetDestination (targetPosition);
            }
            yield return new WaitForSeconds (pathRefreshRate);
        }
    }
}
