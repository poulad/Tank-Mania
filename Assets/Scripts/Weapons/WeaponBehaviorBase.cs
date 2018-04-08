using System;
using UnityEngine;

namespace TankMania
{
    public abstract class WeaponBehaviorBase : MonoBehaviour
    {
        public Sprite WeaponImage;

        public float Scale = 1;

        public event EventHandler<EventArgs> Exploded;

        protected void RaiseExploded(object sender)
        {
            if (Exploded != null)
            {
                Exploded(sender, EventArgs.Empty);
            }
        }
    }
}