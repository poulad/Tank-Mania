using UnityEngine;

namespace TankMania
{
    public class SeaBehavior : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == Constants.Tags.Tank)
            {
                var tankBehavior = other.gameObject.GetComponent<TankBehavior>();
                tankBehavior.TakeDamage(100);
            }
        }
    }
}