using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TankMania
{
    public partial class LevelSceneManagerBehavior
    {
        private Player[] ActivePlayers
        {
            get { return AllPlayers.Where(p => p.Tank && p.TankBehavior.enabled).ToArray(); }
        }

        private Player[] AllPlayers
        {
            get { return GameManager.Current.Players; }
        }

        private Player _currentPlayer;

        private Image PauseMenuPanel;

        private Text _timeoutText;

        private Text _currentPlayerText;

        private Slider _chargeMeterSlider;

        private RawImage _highlightBg;

        private bool _isWaitingForPlayerMove;

        private bool _isPaused;

        private float TurnTimeout = 12f;

        private float _timeout;

        private const float MaxFireCharge = 1.25f;

        private float _fireCharge;

        private readonly IList<string> _losers = new List<string>();

        protected void AssignComponents()
        {
            PauseMenuPanel = ScreenCanvas.GetComponentsInChildren<Image>()
                .Single(c => c.name == "PauseMenu");

            _timeoutText = ScreenCanvas.GetComponentsInChildren<Text>()
                .Single(c => c.name == "TimeoutText");

            _currentPlayerText = ScreenCanvas.GetComponentsInChildren<Text>()
                .Single(c => c.name == "CurrentPlayerText");

            _chargeMeterSlider = ScreenCanvas.GetComponentsInChildren<Slider>()
                .Single(c => c.name == "ChargeMeterSlider");

            _highlightBg = ScreenCanvas.GetComponentsInChildren<RawImage>()
               .Single(c => c.name == "Tank Highlight BG");
        }

        protected void AssignTurnToPlayer(Player player)
        {
            _currentPlayer = player;
            _currentPlayer.TankBehavior.Fired += OnCurrentTankFired;
            _currentPlayer.TankBehavior.TakeCurrentTurn();

            _timeoutText.enabled = true;
            _currentPlayerText.enabled = true;
            _chargeMeterSlider.enabled = true;

            _timeout = TurnTimeout;
            _timeoutText.text = TurnTimeout + "";
            _currentPlayerText.text = _currentPlayer.Name;

            VirtualCamera.Follow = _currentPlayer.Tank.transform;

            SetPlayerTurnActive(false);
        }

        protected void OnTankDestroying(object sender, EventArgs eventArgs)
        {
            var tankBehavior = (TankBehavior)sender;
            var ripPlayer = ActivePlayers
                .Single(p => p.TankBehavior == tankBehavior);

            if (ripPlayer == _currentPlayer)
            {
                _timeoutText.enabled = false;
                _chargeMeterSlider.enabled = false;
                _chargeMeterSlider.value = 0;
                _fireCharge = 0;
                ChangeTanksTurn();
            }
        }

        protected void OnTankDestroyed(object sender, EventArgs eventArgs)
        {
            var tankBehavior = (TankBehavior)sender;
            var ripPlayer = ActivePlayers
                .Single(p => p.TankBehavior == tankBehavior);

            _losers.Add(ripPlayer.Name);

            Destroy(ripPlayer.Tank);

            if (ActivePlayers.Length == 1)
            {
                GameOver();
            }
        }

        private void OnCurrentTankFired(object sender, EventArgs eventArgs)
        {
            _timeoutText.enabled = false;
            _chargeMeterSlider.enabled = false;
            _fireCharge = 0;

            Invoke("ChangeTanksTurn", 2);
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

            string nextScene = GameManager.Current.CurrentLevel < 3
                ? Constants.Scenes.Scores
                : Constants.Scenes.GameOver;
            GameManager.Current.SwitchToScene(nextScene);
        }

        private void RemoveTurnFromCurrentTank()
        {
            _currentPlayer.TankBehavior.Fired -= OnCurrentTankFired;
            _currentPlayer.TankBehavior.StopTurn();
        }

        private void ChangeTanksTurn()
        {
            RemoveTurnFromCurrentTank();
            _currentPlayerText.enabled = false;
            Invoke("AssignTurnToNextPlayer", 2f);
        }

        protected void AssignTurnToNextPlayer()
        {
            Player[] players = ActivePlayers;
            int currentPlayerIndex = -1;
            for (int i = 0; i < players.Length; i++)
                if (players[i] == _currentPlayer)
                    currentPlayerIndex = i;

            int nextPlayerIndex = (currentPlayerIndex + 1) % players.Length;

            AssignTurnToPlayer(players[nextPlayerIndex]);
        }

        private void CheckForPlayerMove()
        {
            if (
                0 < Mathf.Abs(Input.GetAxis("Horizontal")) ||
                0 < Mathf.Abs(Input.GetAxis("Vertical")) ||
                Input.GetKey(FireKey)
            )
            {
                SetPlayerTurnActive(true);
            }
        }

        private void SetPlayerTurnActive(bool isActive)
        {
            _isWaitingForPlayerMove = !isActive;
            _highlightBg.enabled = !isActive;
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
                _timeoutText.text = secsLeft + "";
            }
        }

        private void CheckFireCharge()
        {
            if (!_chargeMeterSlider.enabled)
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

            _chargeMeterSlider.value = _fireCharge / MaxFireCharge;
        }

        private void CheckForRandomDestroy()
        {
            if (_currentPlayer == null)
                return;

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.K))
            {
                ActivePlayers[Random.Range(0, ActivePlayers.Length)]
                    .TankBehavior
                    .TakeDamage(float.MaxValue);
            }
        }
    }
}
