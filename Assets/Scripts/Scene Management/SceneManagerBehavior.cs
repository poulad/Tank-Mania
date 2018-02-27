using System;
using Cinemachine;
using UnityEngine;

namespace TankMania
{
    public class SceneManagerBehavior : MonoBehaviour
    {
        public GameObject[] Tanks;

        public GameObject[] TanksPrefabs;

        public GameObject VirtualCameraObject;

        private GameObject _currentTank;

        private CinemachineVirtualCamera _virtualCamera;

        public void Start()
        {
            _virtualCamera = VirtualCameraObject.GetComponent<CinemachineVirtualCamera>();

            {// ToDo Testing
                Tanks = new[]
                {
                    Instantiate(TanksPrefabs[0], new Vector3(-1, 2, 0), Quaternion.identity),
                    Instantiate(TanksPrefabs[1], new Vector3(1, 2, 0), Quaternion.identity),
                };

                Tanks[1].GetComponent<SpriteRenderer>().color = Color.red;
            }

            AssignTurnToTank(Tanks[0]);
        }

        private void AssignTurnToTank(GameObject tank)
        {
            _virtualCamera.Follow = tank.transform;
            var tankBehavior = tank.GetComponent<TankBehavior>();
            tankBehavior.Fired += OnCurrentTankFired;
            tankBehavior.TakeCurrentTurn();
            _currentTank = tank;
        }

        private void OnCurrentTankFired(object sender, EventArgs eventArgs)
        {
            Invoke("ChangeTanksTurn", 2);
        }

        private void ChangeTanksTurn()
        {
            DestroyObject(_currentTank.gameObject);
            AssignTurnToTank(Tanks[1]);
        }
    }
}
