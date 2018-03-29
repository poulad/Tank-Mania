using UnityEditor;
using UnityEngine;

namespace TankMania
{
    public class GameState : MonoBehaviour
    {
        public static GameState Current;

        public int CurrentLevel { get; set; }

        public Player[] Players { get; set; }

        public void Start()
        {
            DontDestroyOnLoad(this);
            Current = this;
            Current.Players = new Player[4];
        }

        private GameState()
        {
        }
    }

    public class Player
    {
        public string Name { get; set; }

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
