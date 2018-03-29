using System;
using System.Linq;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankMania
{
    public partial class SceneManagerBehavior
    {
        private Player[] AllPlayers
        {
            get { return GameState.Current.Players; }
        }

        private Player[] ActivePlayers
        {
            get { return AllPlayers.Where(p => p.TankBehavior && p.TankBehavior.enabled).ToArray(); }
        }

        private Player _currentPlayer;

        private float _timeout;

        private const float MaxFireCharge = 1.25f;

        private float _fireCharge;

        private CinemachineVirtualCamera _virtualCamera;

        private void AssignTurnToPlayer(Player player)
        {
            _virtualCamera.Follow = player.Tank.transform;

            _currentPlayer = player;
            _currentPlayer.TankBehavior.Fired += OnCurrentTankFired;
            _currentPlayer.TankBehavior.TakeCurrentTurn();

            TimeoutText.enabled = true;
            CurrentPlayerText.enabled = true;
            ChargeMeterSlider.enabled = true;

            _timeout = 12f;
            CurrentPlayerText.text = _currentPlayer.Name;
        }

        private void OnCurrentTankFired(object sender, EventArgs eventArgs)
        {
            TimeoutText.enabled = false;
            CurrentPlayerText.enabled = false;
            ChargeMeterSlider.enabled = false;
            _fireCharge = 0;
            Invoke("ChangeTanksTurn", 2);
        }

        private void OnTankDestroying(object sender, EventArgs eventArgs)
        {
            var tankBehavior = (TankBehavior)sender;
            var tank = tankBehavior.gameObject;

            if (tank == _currentPlayer.Tank)
            {
                int nextTankIndex = (GetIndexOfActiveTank(tank) + 1);
                ChangeTanksTurnTo(nextTankIndex);
            }

            tankBehavior.enabled = false;

            Destroy(tank);

            if (ActivePlayers.Length == 1)
            {
                Debug.Log("GAME OVER!");
            }
        }

        private void RemoveTurnFromCurrentTank()
        {
            _currentPlayer.TankBehavior.Fired -= OnCurrentTankFired;
            _currentPlayer.TankBehavior.StopTurn();
        }

        private void ChangeTanksTurn()
        {
            int currentPlayerIndex = -1;
            for (int i = 0; i < ActivePlayers.Length; i++)
                if (ActivePlayers[i] == _currentPlayer)
                    currentPlayerIndex = i;

            int nextPlayerIndex = (currentPlayerIndex + 1) % ActivePlayers.Length;

            RemoveTurnFromCurrentTank();
            AssignTurnToPlayer(ActivePlayers[nextPlayerIndex]);
        }

        private void ChangeTanksTurnTo(int index)
        {
            RemoveTurnFromCurrentTank();
            AssignTurnToPlayer(ActivePlayers[index]);
        }

        private int GetIndexOfActiveTank(GameObject tank)
        {
            int tankIndex = -1;
            for (int i = 0; i < ActivePlayers.Length; i++)
                if (ActivePlayers[i].Tank == tank)
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

        private void WatchFireCharge()
        {
            if (!ChargeMeterSlider.enabled)
                return;

            bool holdingFireKey = Input.GetKey(FireKey);
            if (_fireCharge > 0 && !holdingFireKey)
                _currentPlayer.TankBehavior.Fire(_fireCharge);
            else if (holdingFireKey)
            {
                _fireCharge += Time.deltaTime;
                if (_fireCharge > MaxFireCharge)
                {
                    _fireCharge = MaxFireCharge;
                    _currentPlayer.TankBehavior.Fire(_fireCharge);
                }
            }

            ChargeMeterSlider.value = _fireCharge / MaxFireCharge;
        }

        private void CheckForRandomDestroy()
        {
            if (_currentPlayer == null)
                return;

            if (
                Mathf.Abs(_currentPlayer.Tank.transform.position.x) <= 1.5f &&
                Input.GetKey(KeyCode.LeftControl) &&
                Input.GetKeyDown(KeyCode.K)
            )
            {
                ActivePlayers[Random.Range(0, ActivePlayers.Length)]
                    .TankBehavior
                    .TakeDamage(float.MaxValue);
            }
        }
    }
}
