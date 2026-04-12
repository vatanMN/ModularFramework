using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModularFW.Core.HapticService;
using ModularFW.Core.PoolSystem;

namespace MiniGame.TowerDefense {
public class Projectile : MonoBehaviour{
    public System.Action<Projectile> OnDestroyed;
    private Transform target;
    private float speed = 300f;
    private float damage = 10f;
    private Vector3 velocity;
    private float lifeTime = 5f;
    private float age = 0f;

    public void Initialize(Transform t, float dmg, float spd)
    {
        target = t;
        damage = dmg;
        speed = spd;
        // lock direction at launch: aim at current target position
        if (target != null)
        {
            var dir = (target.position - transform.position).normalized;
            velocity = dir * speed;
            // orient projectile if desired
            transform.up = dir;
        }
        else
        {
            // if no target, shoot forward
            velocity = transform.up * speed;
        }
        transform.position += velocity.normalized * 50f; // offset forward to avoid hitting tower
        age = 0f;
    }

    // Called when reused from pool to reset transient state
    public void ResetForReuse()
    {
        target = null;
        velocity = Vector3.zero;
        age = 0f;
        transform.localScale = Vector3.one;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        // move along locked velocity
        float dt = Time.deltaTime;
        transform.position += velocity * dt;
        age += dt;

        // check hit against target (if still exists)
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.position) < 50f)
            {
                var enemy = target.GetComponent<Enemy>();
                if (enemy != null) enemy.ApplyDamage(damage);
                // return to pool if available
                OnDestroyed?.Invoke(this);
                if (PoolingService.Instance != null)
                {
                    PoolingService.Instance.Destroy(PoolEnum.Projectile, this.gameObject);
                }
                else Destroy(this.gameObject);
                return;
            }
        }

        // destroy after lifetime
        if (age >= lifeTime)
        {
            OnDestroyed?.Invoke(this);
            if (PoolingService.Instance != null)
            {
                PoolingService.Instance.Destroy(PoolEnum.Projectile, this.gameObject);
            }
            else Destroy(this.gameObject);
        }
    }
}
}
