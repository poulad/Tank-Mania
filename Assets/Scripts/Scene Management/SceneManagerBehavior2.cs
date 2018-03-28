using System;
using System.Linq;
using Cinemachine;
using UnityEngine;

namespace TankMania
{
    public partial class SceneManagerBehavior
    {
        private GameObject _currentTank;

        private TankBehavior _currentTankBehavior;

        private float _timeout;

        private CinemachineVirtualCamera _virtualCamera;

        private void AssignTurnToTank(GameObject tank)
        {
            _virtualCamera.Follow = tank.transform;
            _currentTankBehavior = tank.GetComponent<TankBehavior>();

            _currentTankBehavior.Fired += OnCurrentTankFired;
            _currentTankBehavior.TakeCurrentTurn();
            _currentTank = tank;

            _timeout = 12f;
        }

        private void OnCurrentTankFired(object sender, EventArgs eventArgs)
        {
            Invoke("ChangeTanksTurn", 2);
        }

        private void OnTankDestroying(object sender, EventArgs eventArgs)
        {
            var tankBehavior = (TankBehavior)sender;
            var tank = tankBehavior.gameObject;

            if (tank == _currentTank)
            {
                int nextTankIndex = (GetIndexOfTank(tank) + 1);
                ChangeTanksTurnTo(nextTankIndex);
            }

            Tanks = Tanks
                .Except(new[] { tank })
                .ToArray();

            Destroy(tank);

            if (Tanks.Length == 1)
            {
                Debug.Log("GAME OVER!");
            }
        }

        private void RemoveTurnFromCurrentTank()
        {
            _currentTankBehavior.Fired -= OnCurrentTankFired;
            _currentTankBehavior.StopTurn();
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

        private void ChangeTanksTurnTo(int index)
        {
            RemoveTurnFromCurrentTank();
            AssignTurnToTank(Tanks[index]);
        }

        private int GetIndexOfTank(GameObject tank)
        {
            int tankIndex = -1;
            for (int i = 0; i < Tanks.Length; i++)
                if (Tanks[i] == tank)
                    tankIndex = i;
            return tankIndex;
        }

        private void UpdateTimer()
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0)
            {
                ChangeTanksTurn();
            }
            else
            {
                var secsLeft = (int)Math.Round(_timeout);
                TimeoutText.text = secsLeft + "";
            }
        }
    }
}
