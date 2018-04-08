using System;

namespace TankMania
{
    public class FiredEventArgs : EventArgs
    {
        public WeaponBehaviorBase Weapon { get; private set; }

        public FiredEventArgs(WeaponBehaviorBase weapon)
        {
            Weapon = weapon;
        }
    }
}
