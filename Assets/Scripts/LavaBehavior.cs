using UnityEngine;

namespace TankMania
{
    public class LavaBehavior : MonoBehaviour
    {
        public void Update()
        {

        }

        public void RaiseLevelBy(float value)
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                transform.localScale.y + value,
                transform.localScale.z
            );
        }
    }
}