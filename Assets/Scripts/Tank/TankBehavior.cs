﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TankMania
{
    public partial class TankBehavior : MonoBehaviour
    {
        public string PlayerName;

        public Rigidbody2D ShellPrefab;

        public KeyCode FireKey = KeyCode.Space;

        [HideInInspector]
        public bool HasCurrentTurn { get; private set; }

        public event EventHandler<EventArgs> Fired;

        public event EventHandler<EventArgs> Destroying;

        public void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            _muzzle = GetComponentsInChildren<Transform>()
                .Single(t => t.gameObject.name == "Muzzle")
                .gameObject;
            _launchPoint = GetComponentsInChildren<Transform>()
                .Single(t => t.gameObject.name == "Launch Point")
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

            { // Prevent the health bar from mirroring when tank turns
                _slider.transform.localScale = new Vector3(
                    Math.Abs(_slider.transform.localScale.x) * Math.Sign(this.transform.localScale.x),
                    _slider.transform.localScale.y,
                    _slider.transform.localScale.z
                );
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
            if (_slider.value <= 0)
            {
                if (Destroying != null) Destroying(this, EventArgs.Empty);
            }
        }
    }
}
