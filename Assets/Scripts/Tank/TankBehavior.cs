using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TankMania
{
    public partial class TankBehavior : MonoBehaviour
    {
        public Rigidbody2D ShellPrefab;

        public GameObject ExplosionPrefab;

        public float MaxMuzzleAngle;

        public float MinMuzzleAngle;

        [HideInInspector]
        public bool IsPaused
        {
            get { return _isPaused; }
            set { Pause(value); }
        }

        public event EventHandler<FiredEventArgs> Fired;

        public event EventHandler<EventArgs> Destroying;

        public event EventHandler<EventArgs> Destroyed;

        public void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
            _muzzle = GetComponentsInChildren<Transform>()
                .Single(t => t.gameObject.name == "Muzzle")
                .gameObject;
            _launchPoint = GetComponentsInChildren<Transform>()
                .Single(t => t.gameObject.name == "Launch Point")
                .gameObject;
            _slider = GetComponentInChildren<Slider>();
        }

        public void Update()
        {
            if (IsPaused)
                return;

            _audioSource.pitch = Random.Range(.9f, 1.1f);
            if (_hasCurrentTurn)
            {
                ControlMovement();
            }

            { // Prevent the health bar from mirroring when tank turns
                _slider.transform.localScale = new Vector3(
                    Math.Abs(_slider.transform.localScale.x) * Math.Sign(transform.localScale.x),
                    _slider.transform.localScale.y,
                    _slider.transform.localScale.z
                );
            }
        }

        public void TakeCurrentTurn(GameObject weapon)
        {
            _weapon = weapon;
            _hasCurrentTurn = true;
        }

        public void StopTurn()
        {
            _hasCurrentTurn = false;
            StopDriving();
        }

        public void Fire(float charge)
        {
            if (_weapon.tag == Constants.Tags.LandMine)
            {
                DropExplosive();
            }
            else
            {
                LaunchMissile(charge);
            }
        }

        public void TakeDamage(float damage)
        {
            _slider.value -= damage;
            if (_slider.value <= 0)
            {
                if (Destroying != null) Destroying(this, EventArgs.Empty);

                var explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity, transform);
                explosion.GetComponentInChildren<TankExplosionBehavior>()
                    .Finished += OnExplosionFinished;
            }
        }
    }
}
