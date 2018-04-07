using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankMania
{
    public partial class LevelSceneManagerBehavior
    {
        private Player[] ActivePlayers
        {
            get { return AllPlayers.Where(p => p.Tank && p.TankBehavior.enabled).ToArray(); }
        }

        private Player _currentPlayer;

        private bool _isPaused;

        private float _timeout;

        private const float MaxFireCharge = 1.25f;

        private float _fireCharge;

        private readonly IList<string> _losers = new List<string>();

        protected void AssignTurnToPlayer(Player player)
        {
            _currentPlayer = player;
            _currentPlayer.TankBehavior.Fired += OnCurrentTankFired;
            _currentPlayer.TankBehavior.TakeCurrentTurn();

            TimeoutText.enabled = true;
            CurrentPlayerText.enabled = true;
            ChargeMeterSlider.enabled = true;

            _timeout = TurnTimeout;
            CurrentPlayerText.text = _currentPlayer.Name;
            VirtualCamera.Follow = _currentPlayer.Tank.transform;
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
            var ripPlayer = ActivePlayers
                .Single(p => p.TankBehavior == (TankBehavior)sender);

            if (ripPlayer == _currentPlayer)
            {
                ChangeTanksTurn();
            }

            _losers.Add(ripPlayer.Name);
            Destroy(ripPlayer.Tank);

            if (ActivePlayers.Length == 1)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            if (_losers.Count != 3)
                throw new InvalidOperationException();

            AllPlayers
                .Single(p => !_losers.Contains(p.Name))
                .Score += 3;

            AllPlayers
                .Single(p => p.Name == _losers[2])
                .Score += 2;

            AllPlayers
                .Single(p => p.Name == _losers[1])
                .Score += 1;

            GameManager.Current.SwitchToScene(Constants.Scenes.Scores);
        }

        private void RemoveTurnFromCurrentTank()
        {
            _currentPlayer.TankBehavior.Fired -= OnCurrentTankFired;
            _currentPlayer.TankBehavior.StopTurn();
        }

        private void ChangeTanksTurn()
        {
            Player[] players = ActivePlayers;
            int currentPlayerIndex = -1;
            for (int i = 0; i < players.Length; i++)
                if (players[i] == _currentPlayer)
                    currentPlayerIndex = i;

            int nextPlayerIndex = (currentPlayerIndex + 1) % players.Length;

            RemoveTurnFromCurrentTank();
            AssignTurnToPlayer(players[nextPlayerIndex]);
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

        private void CheckFireCharge()
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
                //Mathf.Abs(_currentPlayer.Tank.transform.position.x) <= 1.5f &&
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
