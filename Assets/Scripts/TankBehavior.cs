using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankMania
{
    public class TankBehavior : MonoBehaviour
    {
        public Rigidbody2D ShellPrefab;

        public KeyCode FireKey = KeyCode.Space;

        [HideInInspector]
        public bool HasCurrentTurn { get; private set; }

        public event EventHandler<EventArgs> Fired;

        private GameObject _muzzle;

        private Rigidbody2D _rigidbody;

        private AudioSource _audioSource;

        private bool _alreadyFired;

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();

            _muzzle = GetComponentsInChildren<Transform>()
                .Single(t => t.gameObject.name == "Muzzle")
                .gameObject;
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
            }

            if (!_alreadyFired && Input.GetKey(FireKey))
            {
                Fire();
            }
        }

        private void Fire()
        {
            _alreadyFired = true;

            var shell = Instantiate(ShellPrefab, _muzzle.transform.position, Quaternion.identity);
            shell.transform.localScale = new Vector3(
                shell.transform.localScale.x * Mathf.Sign(transform.localScale.x),
                shell.transform.localScale.y
            );
            shell.velocity = 8 * (transform.localScale.x > 0 ? Vector3.right : Vector3.left);

            if (Fired != null)
                Fired(this, EventArgs.Empty);
        }
    }
}
