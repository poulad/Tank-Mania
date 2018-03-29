using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace TankMania
{
    public partial class SceneManagerBehavior : MonoBehaviour
    {
        public GameObject[] Tanks;

        public GameObject[] TanksPrefabs;

        public GameObject VirtualCameraObject;

        public Text TimeoutText;

        public Text CurrentPlayerText;

        public Slider ChargeMeterSlider;

        public KeyCode FireKey = KeyCode.Space;

        public void Start()
        {
            _virtualCamera = VirtualCameraObject.GetComponent<CinemachineVirtualCamera>();

            {// ToDo Testing
                Tanks = new[]
                {
                    Instantiate(TanksPrefabs[0], new Vector3(-4, 2, 0), Quaternion.identity),
                    Instantiate(TanksPrefabs[1], new Vector3(0, 2, 0), Quaternion.identity),
                };

                foreach (var sr in Tanks[1].GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.color = Color.red;
                }
            }

            for (int i = 0; i < Tanks.Length; i++)
            {
                var tankBehavior = Tanks[i].GetComponent<TankBehavior>();
                tankBehavior.PlayerName = "Player " + i;
                tankBehavior.Destroying += OnTankDestroying;
            }

            AssignTurnToTank(Tanks[0]);
        }

        public void Update()
        {
            UpdateTimer();
            WatchFireCharge();
        }
    }
}
