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
            get { return _allPlayers.Where(p => p.Tank).ToArray(); }
        }

        private Player[] _allPlayers;

        private Player _currentPlayer;

        private Image _pauseMenuPanel;

        private Text _timeoutText;

        private Text _currentPlayerText;

        private Slider _chargeMeterSlider;

        private RawImage _highlightBg;

        private SpriteRenderer _weaponSpriteRenderer;

        private bool _isWaitingForPlayerMove;

        private bool _isPaused;

        private KeyCode _fireKey = KeyCode.Space;

        private float _timeout;

        private const float MaxFireCharge = 1.25f;

        private float _fireCharge;

        private readonly IList<string> _losers = new List<string>();

        private void GetComponentsOnScene()
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

        private void RandomizePlayersOrder()
        {
            int length = GameManager.Current.Players.Length;
            var players = GameManager.Current.Players.ToList();
            for (int i = 0; i < length - 1; i++)
            {
                int index = Random.Range(0, length - 1);
                var p = players[index];
                players.RemoveAt(index);
                players.Add(p);
            }

            _allPlayers = players.ToArray();
        }

        private void InitializeTanks()
        {
            GameManager.Current.InstantiateTanks(transform);
            for (int i = 0; i < _allPlayers.Length; i++)
            {
                var player = _allPlayers[i];
                player.TankBehavior.Destroying += OnTankDestroying;
                player.TankBehavior.Destroyed += OnTankDestroyed;
                player.Tank.name = "Tank - " + player.Name;
                player.Tank.transform.position = InitialPositions[i];
            }
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

        private int _lastPlayerIndex;

        private void AssignTurnToNextPlayer()
        {
            int nextPlayerIndex = _lastPlayerIndex;
            do
            {
                nextPlayerIndex = (nextPlayerIndex + 1) % _allPlayers.Length;
            } while (!_allPlayers[nextPlayerIndex].Tank);

            AssignTurnToPlayer(nextPlayerIndex);
        }

        private void AssignTurnToPlayer(int i)
        {
            _lastPlayerIndex = i;
            _currentPlayer = _allPlayers[i];
            ResetCurrentTurn();
        }

        private void ChangeTanksTurn()
        {
            StopCurrentTurn();
            Invoke("AssignTurnToNextPlayer", 2f);
        }

        private void StopCurrentTurn()
        {
            SetHudVisible(false);
            if (_currentPlayer != null && _currentPlayer.TankBehavior)
            {
                // Current tank might have got himself destroyed at this point
                _currentPlayer.TankBehavior.StopTurn();
                _currentPlayer.TankBehavior.Fired -= OnCurrentTankFired;
            }
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

        #region Scene Elements Control

        private bool _hasCurrentTankFired;

        private void ResetCurrentTurn()
        {
            _currentPlayer.TankBehavior.Fired += OnCurrentTankFired;

            var currentWeaponPrefab = SelectRandomWeapon();
            _currentPlayer.TankBehavior.TakeCurrentTurn(currentWeaponPrefab);

            SetHudVisible(true);
            _hasCurrentTankFired = false;

            VirtualCamera.Follow = _currentPlayer.Tank.transform;
            SetCurrentTurnActive(false);
        }

        private void SetCurrentTurnActive(bool isActive)
        {
            _isWaitingForPlayerMove = !isActive;
            _highlightBg.enabled = !isActive;
        }

        #endregion

        #region Frame Update Checks

        private void CheckForPlayerMove()
        {
            if (
                0 < Mathf.Abs(Input.GetAxis("Horizontal")) ||
                0 < Mathf.Abs(Input.GetAxis("Vertical")) ||
                Input.GetKey(_fireKey)
            )
            {
                SetCurrentTurnActive(true);
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

            bool holdingFireKey = Input.GetKey(_fireKey);
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

            _allPlayers
                .Single(p => !_losers.Contains(p.Name))
                .Score += 3;

            _allPlayers
                .Single(p => p.Name == _losers[2])
                .Score += 2;

            _allPlayers
                .Single(p => p.Name == _losers[1])
                .Score += 1;

            string nextScene = GameManager.Current.CurrentLevel < 3
                ? Constants.Scenes.Scores
                : Constants.Scenes.GameOver;
            GameManager.Current.SwitchToScene(nextScene);
        }
    }
}
