using System.Linq;
using TankMania.Helpers;
using UnityEngine;

namespace TankMania
{
    public class ShellBehavior : MonoBehaviour
    {
        public float ExplosionRadius = .5f;

        public float ExplosionForce = 500f;

        public float MaxDamage = 100f;

        public LayerMask DestructibleLayer;

        public void Start()
        {
            Destroy(gameObject, 3);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            var targets = Physics2D
                .OverlapCircleAll(transform.position, ExplosionRadius, DestructibleLayer)
                .Select(c => c.gameObject)
                .Distinct()
                .Select(gameObj => new
                {
                    GameObject = gameObj,
                    Rigidbody2D = gameObj.GetComponent<Rigidbody2D>()
                })
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

            Destroy(gameObject);
        }
    }
}