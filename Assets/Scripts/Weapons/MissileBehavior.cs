using System;
using System.Linq;
using TankMania.Helpers;
using UnityEngine;

namespace TankMania
{
    public class MissileBehavior : WeaponBehaviorBase
    {
        public float ExplosionRadius;

        public float ExplosionForce;

        public float MaxDamage;

        public GameObject ExplosionPrefab;

        public float ExplosionScale = 1;

        private Rigidbody2D _rigidbody;

        public void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            Destroy(gameObject, 8);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Constants.Tags.WorldBoundary))
            {
                RaiseExplodedEvent();
                return;
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                RaiseExplodedEvent();
                return;
            }
            else if (other.gameObject.layer != gameObject.layer)
                return;

            var targets = Physics2D
                .OverlapCircleAll(transform.position, ExplosionRadius)
                .Select(c => c.gameObject)
                .Distinct()
                .Except(new[] { gameObject })
                .Where(gObj => gObj.layer == gameObject.layer)
                .ToArray();

            foreach (var target in targets)
            {
                if (target.CompareTag(Constants.Tags.Tank))
                {
                    var rbody = target.GetComponent<Rigidbody2D>();
                    var tankBehavior = target.GetComponent<TankBehavior>();

                    rbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, mode: ForceMode2D.Impulse);
                    tankBehavior.TakeDamage(MaxDamage);
                }
            }

            if (targets.Any())
            {
                ExplodeWithAnimations();
            }
        }

        private void ExplodeWithAnimations()
        {
            _rigidbody.gravityScale = 0;
            _rigidbody.velocity = Vector2.zero;

            var explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity, transform);
            explosion.transform.localScale *= ExplosionScale;
            explosion.GetComponentInChildren<TankExplosionBehavior>()
                .Finished += OnExplosionFinished;
        }

        private void OnExplosionFinished(object sender, EventArgs e)
        {
            RaiseExplodedEvent();
        }

        private void RaiseExplodedEvent()
        {
            RaiseExploded(this);
        }
    }
}