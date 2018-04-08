using System.Linq;
using TankMania.Helpers;
using UnityEngine;

namespace TankMania
{
    public class ShellBehavior : WeaponBehaviorBase
    {
        public float ExplosionRadius = .5f;

        public float ExplosionForce;

        public float Damage;

        private Rigidbody2D _rigidbody;

        public void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            Destroy(gameObject, 8);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Constants.Tags.WorldBoundary))
                ExplodeImmediately();

            if (other.gameObject.layer != gameObject.layer)
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
                    tankBehavior.TakeDamage(Damage);
                }
            }

            if (targets.Any())
            {
                ExplodeWithAnimations();
            }
        }

        private void ExplodeImmediately()
        {
            RaiseExplodedEvent();
        }

        private void ExplodeWithAnimations()
        {
            _rigidbody.gravityScale = 0;
            _rigidbody.velocity = Vector2.zero;

            // ToDo Play Explosion Animation + Sound

            Invoke("RaiseExplodedEvent", 5);
        }

        private void RaiseExplodedEvent()
        {
            RaiseExploded(this);
        }
    }
}