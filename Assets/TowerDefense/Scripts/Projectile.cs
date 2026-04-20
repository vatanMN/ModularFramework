using UnityEngine;
using ModularFW.Core.PoolSystem;

namespace MiniGame.TowerDefense
{
    public class Projectile : MonoBehaviour
    {
        public System.Action<Projectile> OnDestroyed;
        private Transform target;
        private IDamageable cachedTarget;
        private float speed = 300f;
        private float damage = 10f;
        private Vector3 velocity;
        private float lifeTime = 5f;
        private float age = 0f;

        public void Initialize(Transform targetTransform, float dmg, float spd)
        {
            target = targetTransform;
            cachedTarget = target != null ? target.GetComponent<IDamageable>() : null;
            damage = dmg;
            speed = spd;

            if (target != null)
            {
                var dir = (target.position - transform.position).normalized;
                velocity = dir * speed;
                transform.up = dir;
            }
            else
            {
                velocity = transform.up * speed;
            }
            transform.position += velocity.normalized * 50f;
            age = 0f;
        }

        public void ResetForReuse()
        {
            target = null;
            cachedTarget = null;
            velocity = Vector3.zero;
            age = 0f;
            transform.localScale = Vector3.one;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        void Update()
        {
            transform.position += velocity * Time.deltaTime;
            age += Time.deltaTime;

            if (target != null && Vector3.Distance(transform.position, target.position) < 50f)
            {
                cachedTarget?.TakeDamage(damage);
                ReturnToPool();
                return;
            }

            if (age >= lifeTime)
            {
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            OnDestroyed?.Invoke(this);
            if (PoolingService.Instance != null)
                PoolingService.Instance.Destroy(PoolEnum.Projectile, this.gameObject);
            else
                Destroy(this.gameObject);
        }
    }
}
