using UnityEngine;

namespace TankMania
{
    public class DestructibleBehavior : MonoBehaviour
    {
        public void Destruct()
        {
            Destroy(gameObject);
        }
    }
}