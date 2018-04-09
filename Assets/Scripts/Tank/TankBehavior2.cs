using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TankMania
{
    public partial class TankBehavior
    {
        private GameObject _muzzle;

        private GameObject _launchPoint;

        private GameObject _weapon;

        private Rigidbody2D _rigidbody;

        private AudioSource _engineAudioSource;

        private AudioSource _wheelsAudioSource;

        private Animator _animator;

        private Slider _slider;

        private Image _healthFillImage;

        private bool _hasCurrentTurn;

        private bool _isPaused;

        private bool _isDriving;

        private void GetComponentsOnScene()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            var audioSources = GetComponents<AudioSource>();
            _engineAudioSource = audioSources[0];
            _wheelsAudioSource = audioSources[1];

            _animator = GetComponent<Animator>();

            _muzzle = GetComponentsInChildren<Transform>()
                .Single(t => t.gameObject.name == "Muzzle")
                .gameObject;

            _launchPoint = GetComponentsInChildren<Transform>()
                .Single(t => t.gameObject.name == "Launch Point")
                .gameObject;

            _slider = GetComponentInChildren<Slider>();

            _healthFillImage = _slider.GetComponentsInChildren<Image>().Last();
        }

        private void OnExplosionFinished(object sender, EventArgs eventArgs)
        {
            if (Destroyed != null)
                Destroyed(this, EventArgs.Empty);
        }

        private void ControlMovement()
        {
            float moveH = Input.GetAxis("Horizontal");
            if (0 < Mathf.Abs(moveH))
            {
                _rigidbody.velocity = new Vector2(moveH, _rigidbody.velocity.y);

                transform.localScale = new Vector3(
                    transform.localScale.x * Mathf.Sign(moveH * transform.localScale.x),
                    transform.localScale.y
                );

                StartDriving();
            }
            else
            {
                StopDriving();
            }

            float moveV = Input.GetAxis("Vertical");
            if (0 < Mathf.Abs(moveV))
            {
                var rotationAxis = new Vector3(0, 0, Math.Sign(transform.localScale.x));
                _muzzle.transform.Rotate(rotationAxis, Mathf.Sign(moveV));

                float angle = _muzzle.transform.localEulerAngles.z < 180
                    ? _muzzle.transform.localEulerAngles.z
                    : _muzzle.transform.localEulerAngles.z - 360
                ;

                if (angle > MaxMuzzleAngle)
                    _muzzle.transform.localRotation = Quaternion.Euler(0, 0, MaxMuzzleAngle);
                else if (angle < MinMuzzleAngle)
                    _muzzle.transform.localRotation = Quaternion.Euler(0, 0, MinMuzzleAngle);
            }
        }

        private void ControlSounds()
        {
            _engineAudioSource.pitch = _hasCurrentTurn
                ? Random.Range(.8f, 1)
                : Random.Range(.5f, .6f);

            if (_isDriving && !_wheelsAudioSource.isPlaying)
                _wheelsAudioSource.Play();
            else if (!_isDriving && _wheelsAudioSource.isPlaying)
                _wheelsAudioSource.Pause();
        }

        private void Pause(bool pause)
        {
            _isPaused = pause;

            if (pause)
            {
                _engineAudioSource.Pause();
                StopDriving();
            }
            else
            {
                _engineAudioSource.UnPause();
            }
        }

        private void StartDriving()
        {
            _isDriving = true;
            _animator.SetBool("IsDriving", true);
        }

        private void StopDriving()
        {
            _isDriving = false;
            _animator.SetBool("IsDriving", false);
        }

        private void LaunchMissile(float charge)
        {
            var shell = Instantiate(_weapon, _launchPoint.transform.position, Quaternion.identity)
                .GetComponent<Rigidbody2D>();
            shell.transform.localScale = new Vector3(
                shell.transform.localScale.x * Mathf.Sign(transform.localScale.x),
                shell.transform.localScale.y
            );

            var launchDirection = (_launchPoint.transform.position - _muzzle.transform.position).normalized;
            shell.velocity = (5 + charge * 3) * launchDirection;

            var weaponBehavior = shell.GetComponent<WeaponBehaviorBase>();

            if (Fired != null)
                Fired(this, new FiredEventArgs(weaponBehavior));
        }

        private void DropExplosive()
        {
            var dropPoint = new Vector3(
                _launchPoint.transform.position.x,
                _muzzle.transform.position.y,
                _launchPoint.transform.position.z
            );
            var explosive = Instantiate(_weapon, dropPoint, Quaternion.identity)
                .GetComponent<Rigidbody2D>();

            var launchDirection = (_launchPoint.transform.position - _muzzle.transform.position).normalized;
            explosive.velocity = launchDirection * .5f;

            var weaponBehavior = explosive.GetComponent<WeaponBehaviorBase>();

            if (Fired != null)
                Fired(this, new FiredEventArgs(weaponBehavior));
        }

        private void UpdateHealth(float health)
        {
            health = Mathf.Max(0, health);
            _slider.value = health;
            if (Mathf.Approximately(health, 0))
            {
                if (Destroying != null) Destroying(this, EventArgs.Empty);

                var explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity, transform);
                explosion.GetComponentInChildren<TankExplosionBehavior>()
                    .Finished += OnExplosionFinished;
            }

            _healthFillImage.color = Color.Lerp(Color.red, Color.green, health / 100);
        }
    }
}
