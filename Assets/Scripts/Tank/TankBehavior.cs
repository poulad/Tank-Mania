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

        public KeyCode FireKey = KeyCode.Space;

        [HideInInspector]
        public bool HasCurrentTurn { get; private set; }

        public event EventHandler<EventArgs> Fired;

        public void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            _muzzle = GetComponentsInChildren<Transform>()
                .Single(t => t.gameObject.name == "Muzzle")
                .gameObject;
            _slider = GetComponentInChildren<Slider>();
        }

        public void FixedUpdate()
        {
            _audioSource.pitch = Random.Range(.9f, 1.1f);
            if (HasCurrentTurn)
            {
                ControlMovement();
            }
        }

        public void TakeCurrentTurn()
        {
            HasCurrentTurn = true;
            _alreadyFired = false;
        }

        public void StopTurn()
        {
            HasCurrentTurn = false;
        }

        public void TakeDamage(float damage)
        {
            _slider.value -= damage;
        }
    }
}
