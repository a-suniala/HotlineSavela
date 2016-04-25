using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

    public float startingHealth;

    public float Health { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start () {
        Health = startingHealth;
        dead = false;
    }

    public virtual void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection) {
        TakeDamage (damage);
    }

    public virtual void TakeDamage (float damage) {
        Health -= damage;

        if (Health <= 0 && !dead) {
            Die ();
        }
    }

    [ContextMenu ("Self Destruct")]
    protected virtual void Die() {
        dead = true;
        if (OnDeath != null) OnDeath ();
        Destroy (gameObject);
    }

}
