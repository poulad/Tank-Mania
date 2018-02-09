using UnityEngine;

namespace TankMania
{
    public class Sea : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("Other has tag of: " + other.tag);
            if (other.tag.Equals(Constants.Tags.Tank))
            {
                Destroy(other.gameObject, .5f);
            }
        }
    }
}