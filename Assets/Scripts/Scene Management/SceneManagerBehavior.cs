using Cinemachine;
using UnityEngine;

namespace TankMania
{
    public partial class SceneManagerBehavior : MonoBehaviour
    {
        public GameObject[] Tanks;

        public GameObject[] TanksPrefabs;

        public GameObject VirtualCameraObject;

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

            foreach (var tank in Tanks)
            {
                var tankBehavior = tank.GetComponent<TankBehavior>();
                tankBehavior.Destroying += OnTankDestroying;
            }
            AssignTurnToTank(Tanks[0]);
        }
    }
}
