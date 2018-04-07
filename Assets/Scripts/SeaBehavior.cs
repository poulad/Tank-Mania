using UnityEngine;

namespace TankMania
{
    public class SeaBehavior : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Constants.Tags.Tank))
            {
                var tankBehavior = other.GetComponent<TankBehavior>();
                tankBehavior.TakeDamage(100);
            }
            else if (other.CompareTag(Constants.Tags.Shell))
            {
                // ToDo Play water drop sound
                Destroy(other.gameObject);
            }
        }
    }
}