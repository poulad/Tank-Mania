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
            get { return AllPlayers.Where(p => p.Tank).ToArray(); }
        }

        private Player[] AllPlayers
        {
            get { return GameManager.Current.Players; }
        }

        private Player _currentPlayer;

        private Image _pauseMenuPanel;

        private Text _timeoutText;

        private Text _currentPlayerText;

        private Slider _chargeMeterSlider;

        private RawImage _highlightBg;

        private SpriteRenderer _weaponSpriteRenderer;

        private bool _isWaitingForPlayerMove;

        private bool _isPaused;

        private bool _hasCurrentTankFired;

        private float TurnTimeout = 5f;

        private float _timeout;

        private const float MaxFireCharge = 1.25f;

        private float _fireCharge;

        private readonly IList<string> _losers = new List<string>();

        private void AssignComponents()
        {
            _pauseMenuPanel = ScreenCanvas.GetComponentsInChildren<Image>()
                .Single(c => c.name == "PauseMenu");

            _timeoutText = ScreenCanvas.GetComponentsInChildren<Text>()
                .Single(c => c.name == "TimeoutText");

            _currentPlayerText = ScreenCanvas.GetComponentsInChildren<Text>()
                .Single(c => c.name == "CurrentPlayerText");

            _chargeMeterSlider = ScreenCanvas.GetComponentsInChildren<Slider>()
                .Single(c => c.name == "ChargeMeterSlider");

            _highlightBg = ScreenCanvas.GetComponentsInChildren<RawImage>()
               .Single(c => c.name == "Tank Highlight BG");

            _weaponSpriteRenderer = ScreenCanvas.GetComponentsInChildren<Image>()
                .Single(c => c.name == "Weapon Circle")
                .GetComponentInChildren<SpriteRenderer>();
        }

        #region Event Handlers

        private void OnTankDestroying(object sender, EventArgs eventArgs)
        {
            var tankBehavior = (TankBehavior)sender;
            var ripPlayer = ActivePlayers
                .Single(p => p.TankBehavior == tankBehavior);

            if (ripPlayer == _currentPlayer)
            {
                SetHudVisible(false);
                if (ActivePlayers.Length > 2)
                    ChangeTanksTurn();
            }
        }

        private void OnTankDestroyed(object sender, EventArgs eventArgs)
        {
            var tankBehavior = (TankBehavior)sender;
            var ripPlayer = ActivePlayers
                .Single(p => p.TankBehavior == tankBehavior);

            _losers.Add(ripPlayer.Name);

            if (ActivePlayers.Length == 2)
            {
                GameOver();
            }

            Destroy(ripPlayer.Tank);
        }

        private void OnCurrentTankFired(object sender, FiredEventArgs eventArgs)
        {
            _hasCurrentTankFired = true;
            SetHudVisible(false);
            Invoke("StopCurrentTurn", 2);

            eventArgs.Weapon.Exploded += OnCurrentWeaponExploded;
            VirtualCamera.Follow = eventArgs.Weapon.transform;
        }

        private void OnCurrentWeaponExploded(object sender, EventArgs eventArgs)
        {
            VirtualCamera.Follow = _currentPlayer.Tank.transform;
            ChangeTanksTurn();

            var weapon = (WeaponBehaviorBase)sender;
            Destroy(weapon.gameObject);
        }

        #endregion

        #region Tank Turn Control

        private void AssignTurnToPlayer(Player player)
        {
            _currentPlayer = player;
            _currentPlayer.TankBehavior.Fired += OnCurrentTankFired;

            var currentWeaponPrefab = SelectRandomWeapon();
            _currentPlayer.TankBehavior.TakeCurrentTurn(currentWeaponPrefab);

            SetHudVisible(true);
            _hasCurrentTankFired = false;

            VirtualCamera.Follow = _currentPlayer.Tank.transform;
            SetPlayerTurnActive(false);
        }

        private void ChangeTanksTurn()
        {
            StopCurrentTurn();
            Invoke("AssignTurnToNextPlayer", 4f);
        }

        private void StopCurrentTurn()
        {
            SetHudVisible(false);
            _currentPlayer.TankBehavior.StopTurn();
            _currentPlayer.TankBehavior.Fired -= OnCurrentTankFired;
        }

        private void AssignTurnToNextPlayer()
        {
            Player[] players = ActivePlayers;
            int currentPlayerIndex = -1;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == _currentPlayer)
                {
                    currentPlayerIndex = i;
                    break;
                }
            }

            int nextPlayerIndex = (currentPlayerIndex + 1) % players.Length;

            AssignTurnToPlayer(players[nextPlayerIndex]);
        }

        private void SetPlayerTurnActive(bool isActive)
        {
            _isWaitingForPlayerMove = !isActive;
            _highlightBg.enabled = !isActive;
        }

        #endregion

        #region Weapon Control

        private GameObject SelectRandomWeapon()
        {
            var prefab = WeaponPrefabs[Random.Range(0, WeaponPrefabs.Length)];
            var weaponBehavior = prefab.GetComponent<WeaponBehaviorBase>();

            _weaponSpriteRenderer.sprite = weaponBehavior.WeaponImage;
            _weaponSpriteRenderer.transform.localScale =
                new Vector3(weaponBehavior.Scale, weaponBehavior.Scale, 1);

            return prefab;
        }

        private bool FireWeaponIfCharged()
        {
            if (_hasCurrentTankFired)
                return false;

            bool fired = false;
            if (Mathf.Epsilon < _fireCharge)
            {
                _currentPlayer.TankBehavior.Fire(_fireCharge);
                _fireCharge = 0;
                fired = true;
            }

            return fired;
        }

        #endregion

        #region Frame Update Checks

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

        private void UpdateTimer()
        {
            if (_timeout < 0)
                return;

            _timeout -= Time.deltaTime;
            if (_timeout <= 0)
            {
                bool fired = FireWeaponIfCharged();
                if (!fired)
                {
                    ChangeTanksTurn();
                }
            }
            else
            {
                var secsLeft = (int)Math.Round(_timeout);
                _timeoutText.text = secsLeft + "";
            }
        }

        private void CheckFireCharge()
        {
            if (_hasCurrentTankFired)
                return;

            bool holdingFireKey = Input.GetKey(FireKey);
            if (holdingFireKey)
            {
                _fireCharge += Time.deltaTime;
                if (_fireCharge > MaxFireCharge)
                {
                    _fireCharge = MaxFireCharge;
                    _currentPlayer.TankBehavior.Fire(_fireCharge);
                }
            }
            else
            {
                FireWeaponIfCharged();
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

        #endregion

        private void SetHudVisible(bool active)
        {
            _timeout = active
                ? TurnTimeout
                : -1;

            _timeoutText.text = _timeout + "";
            _timeoutText.enabled = active;

            _currentPlayerText.enabled = active;
            _chargeMeterSlider.enabled = active;

            _currentPlayerText.text = _currentPlayer.Name;

            _fireCharge = 0;
            _chargeMeterSlider.value = 0;
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
    }
}
