using UnityEngine;

namespace TankMania
{
    public class Player
    {
        public string Name { get; set; }

        public int Score { get; set; }

        public GameObject Tank { get; set; }

        public TankBehavior TankBehavior
        {
            get
            {
                if (Tank == null)
                    return null;

                if (_tankBehavior == null)
                    _tankBehavior = Tank.GetComponent<TankBehavior>();
                return _tankBehavior;
            }
        }

        public GameObject TankPrefab { get; set; }

        private TankBehavior _tankBehavior;
    }
}