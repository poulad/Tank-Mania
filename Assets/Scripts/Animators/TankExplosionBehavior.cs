using System;
using UnityEngine;

namespace TankMania
{
    public class TankExplosionBehavior : MonoBehaviour
    {
        public event EventHandler<EventArgs> Finished;

        public void FinishExplosion()
        {
            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }
    }
}