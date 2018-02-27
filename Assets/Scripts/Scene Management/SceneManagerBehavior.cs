using System;
using Cinemachine;
using UnityEngine;

namespace TankMania
{
    public class SceneManagerBehavior : MonoBehaviour
    {
        public GameObject[] Tanks;

        public GameObject VirtualCameraObject;

        private GameObject _currentTank;

        private CinemachineVirtualCamera _virtualCamera;

        public void Start()
        {
            _virtualCamera = VirtualCameraObject.GetComponent<CinemachineVirtualCamera>();
            AssignTurnToTank(Tanks[0]);
        }

        private void AssignTurnToTank(GameObject tank)
        {
            _virtualCamera.Follow = tank.transform;
            var tankBehavior = tank.GetComponent<TankBehavior>();
            tankBehavior.Fired += OnTankFired;
            tankBehavior.TakeCurrentTurn();
            _currentTank = tank;
        }

        private void OnTankFired(object sender, EventArgs eventArgs)
        {
            Invoke("ChangeTanksTurn", 2);
        }

        private void ChangeTanksTurn()
        {
            DestroyObject(_currentTank.gameObject);
        }
    }
}
