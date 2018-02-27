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

        private TankBehavior _currentTankBehavior;

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
            _currentTankBehavior = tank.GetComponent<TankBehavior>();

            _currentTankBehavior.Fired += OnCurrentTankFired;
            _currentTankBehavior.TakeCurrentTurn();
            _currentTank = tank;
        }

        private void RemoveTurnFromCurrentTank()
        {
            _currentTankBehavior.Fired -= OnCurrentTankFired;
            _currentTankBehavior.StopTurn();
        }

        private void OnCurrentTankFired(object sender, EventArgs eventArgs)
        {
            Invoke("ChangeTanksTurn", 2);
        }

        private void ChangeTanksTurn()
        {
            int currentTankIndex = -1;
            for (int i = 0; i < Tanks.Length; i++)
                if (Tanks[i] == _currentTank)
                    currentTankIndex = i;

            int nextTankIndex = (currentTankIndex + 1) % Tanks.Length;

            RemoveTurnFromCurrentTank();
            AssignTurnToTank(Tanks[nextTankIndex]);
        }
    }
}
