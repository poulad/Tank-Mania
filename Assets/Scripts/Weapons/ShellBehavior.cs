using System.Linq;
using TankMania.Helpers;
using UnityEngine;

namespace TankMania
{
    public class ShellBehavior : WeaponBehaviorBase
    {
        public float ExplosionRadius = .5f;

        public float ExplosionForce = 500f;

        public float MaxDamage = 100f;

        public void Start()
        {
            Destroy(gameObject, 3);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Constants.Tags.WorldBoundary))
                Destroy(gameObject);

            if (other.gameObject.layer != gameObject.layer)
                return;

            var targets = Physics2D
                .OverlapCircleAll(transform.position, ExplosionRadius)
                .Select(c => c.gameObject)
                .Distinct()
                .Except(new[] { gameObject })
                .Where(gObj => gObj.layer == gameObject.layer)
                .Select(gObj => new
                {
                    GameObject = gObj,
                    Rigidbody2D = gObj.GetComponent<Rigidbody2D>()
                })
                .Where(gObj => gObj.Rigidbody2D)
                .ToArray();

            foreach (var target in targets)
            {
                target.Rigidbody2D.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, mode: ForceMode2D.Impulse);
                if (target.GameObject.tag == Constants.Tags.Tank)
                {
                    var tankBehavior = target.GameObject.GetComponent<TankBehavior>();
                    tankBehavior.TakeDamage(20);
                }
            }

            if (targets.Any())
                Destroy(gameObject);
        }
    }
}